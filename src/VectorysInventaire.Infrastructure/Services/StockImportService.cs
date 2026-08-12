using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Infrastructure.Persistence;

namespace VectorysInventaire.Infrastructure.Services;

public class StockImportService : IStockImportService
{
    private const int BatchSize = 1000;

    private readonly VectorysDbContext _db;
    private readonly IExcelImportService _excelImportService;

    public StockImportService(VectorysDbContext db, IExcelImportService excelImportService)
    {
        _db = db;
        _excelImportService = excelImportService;
    }

    public async Task<ImportStockResultDto> ImportForPassageAsync(
        int passageId,
        Stream fileStream,
        IReadOnlyList<string>? defaultCodeAgents = null,
        CancellationToken cancellationToken = default)
    {
        _ = defaultCodeAgents;

        var passage = await _db.Passages.FirstOrDefaultAsync(p => p.Id == passageId, cancellationToken)
            ?? throw new KeyNotFoundException("Passage introuvable.");

        var agents = await _db.Utilisateurs.AsNoTracking()
            .Where(u => u.CodePersonnel != null)
            .ToDictionaryAsync(u => u.CodePersonnel!, u => u.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

        var rows = await _excelImportService.ParseStockFileAsync(fileStream, cancellationToken);
        if (rows.Count == 0)
            throw new InvalidOperationException("Le fichier Excel ne contient aucune ligne de données.");

        var validationErrors = new List<string>();
        var unknownAgents = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var emplacementsByCode = new Dictionary<string, Emplacement>(StringComparer.OrdinalIgnoreCase);
        var agentCodeByEmplacement = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var stocks = new List<StockTheorique>();

        foreach (var row in rows)
        {
            if (!emplacementsByCode.TryGetValue(row.CodeEmplacement, out var emplacement))
            {
                if (string.IsNullOrWhiteSpace(row.CodeAgentPersonnel))
                {
                    validationErrors.Add(
                        $"Ligne {row.RowNumber} : la colonne CodeAgent est obligatoire pour l'emplacement « {row.CodeEmplacement} ».");
                    continue;
                }

                var codeAgent = row.CodeAgentPersonnel.Trim();
                if (!agents.ContainsKey(codeAgent))
                {
                    unknownAgents.Add(codeAgent);
                    continue;
                }

                emplacement = new Emplacement
                {
                    PassageId = passage.Id,
                    Code = row.CodeEmplacement,
                    Designation = row.DesignationEmplacement ?? row.CodeEmplacement,
                    CodeBarres = row.CodeEmplacement,
                    Statut = StatutEmplacementEnum.EN_ATTENTE,
                    AgentAssigneId = agents[codeAgent]
                };
                emplacementsByCode[row.CodeEmplacement] = emplacement;
                agentCodeByEmplacement[row.CodeEmplacement] = codeAgent;
            }
            else if (!string.IsNullOrWhiteSpace(row.CodeAgentPersonnel))
            {
                var expectedCode = agentCodeByEmplacement[row.CodeEmplacement];
                if (!string.Equals(expectedCode, row.CodeAgentPersonnel.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    validationErrors.Add(
                        $"Ligne {row.RowNumber} : CodeAgent incohérent pour « {row.CodeEmplacement} » " +
                        $"(attendu « {expectedCode} », reçu « {row.CodeAgentPersonnel.Trim()} »).");
                    continue;
                }
            }

            stocks.Add(new StockTheorique
            {
                Emplacement = emplacement,
                CodeUS = row.CodeUS,
                DesignationUS = row.DesignationUS ?? row.CodeUS,
                QteTheorique = row.QteTheorique,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (unknownAgents.Count > 0)
        {
            throw new InvalidOperationException(
                $"Code(s) agent inconnu(s) dans le fichier Excel : {string.Join(", ", unknownAgents.OrderBy(c => c))}.");
        }

        if (validationErrors.Count > 0)
        {
            throw new InvalidOperationException(string.Join(" ", validationErrors.Take(5)) +
                (validationErrors.Count > 5 ? $" (+{validationErrors.Count - 5} autre(s) erreur(s))" : string.Empty));
        }

        if (emplacementsByCode.Count == 0)
            throw new InvalidOperationException("Aucun emplacement valide à importer.");

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        var emplacementList = emplacementsByCode.Values.ToList();
        for (var i = 0; i < emplacementList.Count; i += BatchSize)
        {
            await _db.Emplacements.AddRangeAsync(emplacementList.Skip(i).Take(BatchSize), cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        for (var i = 0; i < stocks.Count; i += BatchSize)
        {
            await _db.StocksTheoriques.AddRangeAsync(stocks.Skip(i).Take(BatchSize), cancellationToken);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var inventaire = await _db.Inventaires.FindAsync([passage.InventaireId], cancellationToken);
        if (inventaire is not null && inventaire.Statut == StatutInventaireEnum.EN_PREPARATION)
            inventaire.Statut = StatutInventaireEnum.EN_COURS;

        passage.Statut = "EN_COURS";
        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var assignationsParAgent = agentCodeByEmplacement.Values
            .GroupBy(code => code, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

        return new ImportStockResultDto
        {
            EmplacementsCrees = emplacementList.Count,
            LignesStockImportees = stocks.Count,
            AssignationsParAgent = assignationsParAgent,
            AgentsInconnus = Array.Empty<string>()
        };
    }
}
