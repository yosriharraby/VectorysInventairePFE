using ClosedXML.Excel;
using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheB;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Infrastructure.Persistence;

namespace VectorysInventaire.Infrastructure.Services;

public class StockImportService : IStockImportService
{
    private const int BatchSize = 1000;
    private const int MaxCodeLength = 256;
    private const int MaxDesignationLength = 500;
    private const int MaxClientLength = 256;

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
        var passage = await _db.Passages.FirstOrDefaultAsync(p => p.Id == passageId, cancellationToken)
            ?? throw new KeyNotFoundException("Passage introuvable.");

        var agents = await LoadAgentsAsync(cancellationToken);
        var (emplacementsByCode, agentCodeByEmplacement, stocks, _) =
            await BuildEmplacementsFromExcelAsync(
                fileStream,
                passage.Id,
                agents,
                allowedCodes: null,
                defaultCodeAgents,
                cancellationToken);

        await PersistImportAsync(passage, emplacementsByCode, stocks, cancellationToken);

        var assignationsParAgent = agentCodeByEmplacement.Values
            .GroupBy(code => code, StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

        return new ImportStockResultDto
        {
            EmplacementsCrees = emplacementsByCode.Count,
            LignesStockImportees = stocks.Count,
            AssignationsParAgent = assignationsParAgent,
            AgentsInconnus = Array.Empty<string>()
        };
    }

    public async Task<AppliquerBrancheBResultDto> ImportBrancheBAsync(
        int inventaireId,
        Stream fileStream,
        string mode,
        string? fichierNom = null,
        string? assignationMode = null,
        string? agentCode = null,
        int? superviseurId = null,
        CancellationToken cancellationToken = default)
    {
        var inventaire = await _db.Inventaires.FirstOrDefaultAsync(i => i.Id == inventaireId, cancellationToken)
            ?? throw new KeyNotFoundException("Inventaire introuvable.");

        if (inventaire.Statut == StatutInventaireEnum.CLOTURE)
            throw new InvalidOperationException("Impossible d'appliquer la Branche B : inventaire déjà clôturé.");

        var passageC1 = await _db.Passages.FirstOrDefaultAsync(
            p => p.InventaireId == inventaireId && p.Type == TypePassageEnum.C1, cancellationToken)
            ?? throw new KeyNotFoundException("Passage C1 introuvable.");

        var emplacementsC1 = await _db.Emplacements.AsNoTracking()
            .Include(e => e.AgentAssigne)
            .Where(e => e.PassageId == passageC1.Id)
            .ToListAsync(cancellationToken);

        if (emplacementsC1.Count == 0)
            throw new InvalidOperationException("Aucun emplacement dans le passage C1 (import initial manquant).");

        var c1Set = new HashSet<string>(emplacementsC1.Select(e => e.Code), StringComparer.OrdinalIgnoreCase);
        var c1AgentByCode = emplacementsC1.ToDictionary(
            e => e.Code,
            e => e.AgentAssigne?.CodePersonnel ?? string.Empty,
            StringComparer.OrdinalIgnoreCase);

        var normalizedMode = BrancheBModes.Normalize(mode);
        var isCible = string.Equals(normalizedMode, BrancheBModes.Cible, StringComparison.OrdinalIgnoreCase);
        var normalizedAssignation = AssignationRecomptageModes.Normalize(assignationMode, normalizedMode);

        if (string.Equals(normalizedAssignation, AssignationRecomptageModes.NouveauAgent, StringComparison.OrdinalIgnoreCase)
            && string.IsNullOrWhiteSpace(agentCode))
        {
            throw new InvalidOperationException("Veuillez sélectionner un agent pour l'assignation « nouveau agent ».");
        }

        var agents = await LoadAgentsAsync(cancellationToken);
        var allAgentCodes = agents.Keys.OrderBy(c => c).ToList();

        var rows = await _excelImportService.ParseStockFileAsync(fileStream, cancellationToken);
        if (rows.Count == 0)
            throw new InvalidOperationException("Le fichier Excel ne contient aucune ligne de données.");

        var excelCodes = rows
            .Select(r => r.CodeEmplacement.Trim())
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        if (excelCodes.Count == 0)
            throw new InvalidOperationException("Aucun code emplacement valide dans le fichier Excel.");

        if (isCible)
        {
            var inconnus = excelCodes
                .Where(c => !c1Set.Contains(c))
                .OrderBy(c => c)
                .ToList();

            if (inconnus.Count > 0)
            {
                throw new InvalidOperationException(
                    "Branche B (ciblé) : emplacement(s) absent(s) de l'import Excel initial : "
                    + string.Join(", ", inconnus)
                    + ". Le fichier ne peut contenir que des emplacements déjà présents au C1.");
            }
        }

        var forcedAgentByEmplacement = ResolveForcedAgents(
            excelCodes,
            normalizedAssignation,
            agentCode,
            c1AgentByCode,
            allAgentCodes);

        await using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        var passageC2 = await _db.Passages.FirstOrDefaultAsync(
            p => p.InventaireId == inventaireId && p.Type == TypePassageEnum.C2, cancellationToken);

        if (passageC2 is null)
        {
            passageC2 = new Passage
            {
                InventaireId = inventaireId,
                Type = TypePassageEnum.C2,
                Numero = 2,
                Statut = "EN_PREPARATION",
                CreatedAt = DateTime.UtcNow
            };
            _db.Passages.Add(passageC2);
            await _db.SaveChangesAsync(cancellationToken);
        }

        var now = DateTime.UtcNow;
        var recomptage = new Recomptage
        {
            InventaireId = inventaireId,
            PassageC2Id = passageC2.Id,
            Mode = normalizedMode,
            AssignationMode = normalizedAssignation,
            FichierNom = fichierNom,
            SuperviseurId = superviseurId ?? inventaire.SuperviseurId,
            EmplacementsCrees = 0,
            EcartsTraites = 0,
            Statut = RecomptageStatuts.EnAttente,
            CreatedAt = now
        };
        _db.Recomptages.Add(recomptage);
        await _db.SaveChangesAsync(cancellationToken);

        var existingC2Codes = await _db.Emplacements.AsNoTracking()
            .Where(e => e.PassageId == passageC2.Id)
            .Select(e => e.Code)
            .ToListAsync(cancellationToken);
        var existingSet = new HashSet<string>(existingC2Codes, StringComparer.OrdinalIgnoreCase);

        var (emplacementsByCode, agentCodeByEmplacement, stocks, skippedExisting) =
            BuildEmplacementsFromRows(
                rows,
                passageC2.Id,
                agents,
                existingSet,
                isCible ? c1Set : null,
                forcedAgentByEmplacement,
                recomptage.Id,
                fallbackAgentCodes: allAgentCodes);

        await PersistImportAsync(passageC2, emplacementsByCode, stocks, cancellationToken, wrapTransaction: false);

        recomptage.EmplacementsCrees = emplacementsByCode.Count;

        var importedCodes = new HashSet<string>(
            emplacementsByCode.Keys.Concat(skippedExisting),
            StringComparer.OrdinalIgnoreCase);

        recomptage.EcartsTraites = await _db.Ecarts.CountAsync(
            e => e.InventaireId == inventaireId
                 && e.Statut == StatutEcartEnum.DETECTE
                 && importedCodes.Contains(e.CodeEmplacement),
            cancellationToken);

        foreach (var group in agentCodeByEmplacement.Values
                     .GroupBy(c => c, StringComparer.OrdinalIgnoreCase))
        {
            if (!agents.TryGetValue(group.Key, out var agentId))
                continue;

            recomptage.Agents.Add(new RecomptageAgent
            {
                AgentId = agentId,
                EmplacementsAssignes = group.Count()
            });
        }

        await _db.SaveChangesAsync(cancellationToken);
        await transaction.CommitAsync(cancellationToken);

        var modeLabel = isCible ? "ciblé" : "intégral";
        return new AppliquerBrancheBResultDto
        {
            InventaireId = inventaireId,
            PassageC2Id = passageC2.Id,
            RecomptageId = recomptage.Id,
            EcartsTraites = recomptage.EcartsTraites,
            EmplacementsCreesC2 = emplacementsByCode.Count,
            Message =
                $"Recomptage {modeLabel} lancé : {emplacementsByCode.Count} emplacement(s). "
                + "Statut En attente — la correction se fait par le scan de l'agent."
                + (skippedExisting.Count > 0 ? $" {skippedExisting.Count} déjà sur C2 ignoré(s)." : string.Empty)
        };
    }

    public async Task<byte[]> ExportC1StockAsync(int inventaireId, CancellationToken cancellationToken = default)
    {
        var passageC1 = await _db.Passages.AsNoTracking()
            .FirstOrDefaultAsync(p => p.InventaireId == inventaireId && p.Type == TypePassageEnum.C1, cancellationToken)
            ?? throw new KeyNotFoundException("Passage C1 introuvable.");

        var rows = await _db.Emplacements.AsNoTracking()
            .Include(e => e.AgentAssigne)
            .Include(e => e.StocksTheoriques)
            .Where(e => e.PassageId == passageC1.Id)
            .OrderBy(e => e.Code)
            .SelectMany(e => e.StocksTheoriques.Select(s => new
            {
                e.Code,
                e.Designation,
                s.CodeUS,
                s.DesignationUS,
                s.QteTheorique,
                s.Client,
                Agent = e.AgentAssigne != null ? e.AgentAssigne.CodePersonnel : string.Empty
            }))
            .ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Stock");
        string[] headers =
        [
            "CodeEmplacement", "DesignationEmplacement", "CodeUS", "DesignationUS", "QteTheorique", "CodeAgent", "Client"
        ];
        for (var c = 0; c < headers.Length; c++)
            ws.Cell(1, c + 1).Value = headers[c];

        var row = 2;
        foreach (var r in rows)
        {
            ws.Cell(row, 1).Value = r.Code;
            ws.Cell(row, 2).Value = r.Designation;
            ws.Cell(row, 3).Value = r.CodeUS;
            ws.Cell(row, 4).Value = r.DesignationUS;
            ws.Cell(row, 5).Value = r.QteTheorique;
            ws.Cell(row, 6).Value = r.Agent ?? string.Empty;
            ws.Cell(row, 7).Value = r.Client ?? string.Empty;
            row++;
        }

        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<(byte[] Content, string FileName)> ExportRecomptageAsync(
        int recomptageId,
        CancellationToken cancellationToken = default)
    {
        var recomptage = await _db.Recomptages.AsNoTracking()
            .FirstOrDefaultAsync(r => r.Id == recomptageId, cancellationToken)
            ?? throw new KeyNotFoundException("Recomptage introuvable.");

        var rows = await _db.Emplacements.AsNoTracking()
            .Include(e => e.AgentAssigne)
            .Include(e => e.StocksTheoriques)
            .Where(e => e.RecomptageId == recomptageId)
            .OrderBy(e => e.Code)
            .SelectMany(e => e.StocksTheoriques.Select(s => new
            {
                e.Code,
                e.Designation,
                s.CodeUS,
                s.DesignationUS,
                s.QteTheorique,
                s.Client,
                Agent = e.AgentAssigne != null ? e.AgentAssigne.CodePersonnel : string.Empty
            }))
            .ToListAsync(cancellationToken);

        var bytes = WriteStockWorkbook(rows.Select(r => (
            r.Code, r.Designation, r.CodeUS, r.DesignationUS, r.QteTheorique, r.Agent ?? string.Empty, r.Client)));
        var safeName = string.IsNullOrWhiteSpace(recomptage.FichierNom)
            ? $"recomptage-{recomptage.Id}.xlsx"
            : Path.GetFileNameWithoutExtension(recomptage.FichierNom) + "-export.xlsx";
        return (bytes, safeName);
    }

    private static byte[] WriteStockWorkbook(
        IEnumerable<(string Code, string Designation, string CodeUS, string DesignationUS, int QteTheorique, string Agent, string? Client)> rows)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("Stock");
        string[] headers =
        [
            "CodeEmplacement", "DesignationEmplacement", "CodeUS", "DesignationUS", "QteTheorique", "CodeAgent", "Client"
        ];
        for (var c = 0; c < headers.Length; c++)
            ws.Cell(1, c + 1).Value = headers[c];

        var row = 2;
        foreach (var r in rows)
        {
            ws.Cell(row, 1).Value = r.Code;
            ws.Cell(row, 2).Value = r.Designation;
            ws.Cell(row, 3).Value = r.CodeUS;
            ws.Cell(row, 4).Value = r.DesignationUS;
            ws.Cell(row, 5).Value = r.QteTheorique;
            ws.Cell(row, 6).Value = r.Agent;
            ws.Cell(row, 7).Value = r.Client ?? string.Empty;
            row++;
        }

        ws.Columns().AdjustToContents();
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private static Dictionary<string, string> ResolveForcedAgents(
        IReadOnlyList<string> excelCodes,
        string assignationMode,
        string? agentCode,
        Dictionary<string, string> c1AgentByCode,
        IReadOnlyList<string> allAgentCodes)
    {
        if (string.Equals(assignationMode, AssignationRecomptageModes.Excel, StringComparison.OrdinalIgnoreCase))
            return new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        var result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

        if (string.Equals(assignationMode, AssignationRecomptageModes.MemeAgent, StringComparison.OrdinalIgnoreCase))
        {
            foreach (var code in excelCodes)
            {
                if (c1AgentByCode.TryGetValue(code, out var c1Agent) && !string.IsNullOrWhiteSpace(c1Agent))
                    result[code] = c1Agent;
            }
            return result;
        }

        if (string.Equals(assignationMode, AssignationRecomptageModes.NouveauAgent, StringComparison.OrdinalIgnoreCase))
        {
            var code = agentCode!.Trim();
            foreach (var emp in excelCodes)
                result[emp] = code;
            return result;
        }

        if (string.Equals(assignationMode, AssignationRecomptageModes.TousAgents, StringComparison.OrdinalIgnoreCase))
        {
            if (allAgentCodes.Count == 0)
                throw new InvalidOperationException("Aucun agent disponible pour l'assignation.");

            for (var i = 0; i < excelCodes.Count; i++)
                result[excelCodes[i]] = allAgentCodes[i % allAgentCodes.Count];
        }

        return result;
    }

    private async Task<Dictionary<string, int>> LoadAgentsAsync(CancellationToken cancellationToken) =>
        await _db.Utilisateurs.AsNoTracking()
            .Where(u => u.CodePersonnel != null)
            .ToDictionaryAsync(u => u.CodePersonnel!, u => u.Id, StringComparer.OrdinalIgnoreCase, cancellationToken);

    private async Task<(
            Dictionary<string, Emplacement> Emplacements,
            Dictionary<string, string> AgentByEmp,
            List<StockTheorique> Stocks,
            List<string> Skipped)>
        BuildEmplacementsFromExcelAsync(
            Stream fileStream,
            int passageId,
            Dictionary<string, int> agents,
            HashSet<string>? allowedCodes,
            IReadOnlyList<string>? fallbackAgentCodes,
            CancellationToken cancellationToken)
    {
        var rows = await _excelImportService.ParseStockFileAsync(fileStream, cancellationToken);
        if (rows.Count == 0)
            throw new InvalidOperationException("Le fichier Excel ne contient aucune ligne de données.");

        return BuildEmplacementsFromRows(
            rows,
            passageId,
            agents,
            existingOnPassage: null,
            allowedCodes,
            forcedAgentByEmplacement: null,
            recomptageId: null,
            fallbackAgentCodes: fallbackAgentCodes);
    }

    private static (
            Dictionary<string, Emplacement> Emplacements,
            Dictionary<string, string> AgentByEmp,
            List<StockTheorique> Stocks,
            List<string> Skipped)
        BuildEmplacementsFromRows(
            IReadOnlyList<ExcelStockRow> rows,
            int passageId,
            Dictionary<string, int> agents,
            HashSet<string>? existingOnPassage,
            HashSet<string>? allowedCodes,
            Dictionary<string, string>? forcedAgentByEmplacement = null,
            int? recomptageId = null,
            IReadOnlyList<string>? fallbackAgentCodes = null)
    {
        var validationErrors = new List<string>();
        var emplacementsByCode = new Dictionary<string, Emplacement>(StringComparer.OrdinalIgnoreCase);
        var agentCodeByEmplacement = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var stocksByKey = new Dictionary<string, StockTheorique>(StringComparer.OrdinalIgnoreCase);
        var skippedExisting = new List<string>();
        var fallback = ResolveFallbackAgents(agents, fallbackAgentCodes);
        var fallbackIndex = 0;

        foreach (var row in rows)
        {
            var codeEmp = Clip(row.CodeEmplacement, MaxCodeLength);
            if (codeEmp.Length == 0)
                continue;

            if (allowedCodes is not null && !allowedCodes.Contains(codeEmp))
            {
                validationErrors.Add(
                    $"Ligne {row.RowNumber} : emplacement « {codeEmp} » absent de l'import C1 initial.");
                continue;
            }

            if (existingOnPassage is not null && existingOnPassage.Contains(codeEmp))
            {
                if (!skippedExisting.Contains(codeEmp, StringComparer.OrdinalIgnoreCase))
                    skippedExisting.Add(codeEmp);
                continue;
            }

            if (!emplacementsByCode.TryGetValue(codeEmp, out var emplacement))
            {
                string codeAgent;
                if (forcedAgentByEmplacement is not null
                    && forcedAgentByEmplacement.TryGetValue(codeEmp, out var forced)
                    && agents.ContainsKey(forced))
                {
                    codeAgent = forced;
                }
                else
                {
                    codeAgent = ResolveAgentCode(row.CodeAgentPersonnel, agents, fallback, ref fallbackIndex);
                }

                emplacement = new Emplacement
                {
                    PassageId = passageId,
                    Code = codeEmp,
                    Designation = Clip(row.DesignationEmplacement, MaxDesignationLength, codeEmp),
                    CodeBarres = codeEmp,
                    Statut = StatutEmplacementEnum.EN_ATTENTE,
                    AgentAssigneId = agents[codeAgent],
                    RecomptageId = recomptageId
                };
                emplacementsByCode[codeEmp] = emplacement;
                agentCodeByEmplacement[codeEmp] = codeAgent;
            }

            var codeUs = Clip(row.CodeUS, MaxCodeLength, $"{codeEmp}#L{row.RowNumber}");
            var stockKey = $"{codeEmp}\u001f{codeUs}";
            if (stocksByKey.TryGetValue(stockKey, out var existingStock))
            {
                existingStock.QteTheorique += Math.Max(0, row.QteTheorique);
                continue;
            }

            stocksByKey[stockKey] = new StockTheorique
            {
                Emplacement = emplacement,
                CodeUS = codeUs,
                DesignationUS = Clip(row.DesignationUS, MaxDesignationLength, codeUs),
                QteTheorique = Math.Max(0, row.QteTheorique),
                Client = string.IsNullOrWhiteSpace(row.Client) ? null : Clip(row.Client, MaxClientLength),
                CreatedAt = DateTime.UtcNow
            };
        }

        if (validationErrors.Count > 0)
        {
            throw new InvalidOperationException(string.Join(" ", validationErrors.Take(5))
                + (validationErrors.Count > 5 ? $" (+{validationErrors.Count - 5} autre(s) erreur(s))" : string.Empty));
        }

        var stocks = stocksByKey.Values.ToList();

        if (emplacementsByCode.Count == 0 && skippedExisting.Count == 0)
            throw new InvalidOperationException("Aucun emplacement valide à importer.");

        if (emplacementsByCode.Count == 0 && skippedExisting.Count > 0)
            throw new InvalidOperationException(
                "Tous les emplacements du fichier existent déjà sur le passage C2. Aucune création effectuée.");

        return (emplacementsByCode, agentCodeByEmplacement, stocks, skippedExisting);
    }

    private static List<string> ResolveFallbackAgents(
        Dictionary<string, int> agents,
        IReadOnlyList<string>? preferred)
    {
        var fallback = new List<string>();
        if (preferred is not null)
        {
            foreach (var code in preferred)
            {
                if (string.IsNullOrWhiteSpace(code))
                    continue;
                var trimmed = code.Trim();
                if (agents.ContainsKey(trimmed) && !fallback.Contains(trimmed, StringComparer.OrdinalIgnoreCase))
                    fallback.Add(trimmed);
            }
        }

        if (fallback.Count == 0)
            fallback.AddRange(agents.Keys);

        if (fallback.Count == 0)
            throw new InvalidOperationException("Aucun agent avec code personnel n'est disponible pour l'assignation.");

        return fallback;
    }

    private static string ResolveAgentCode(
        string? requested,
        Dictionary<string, int> agents,
        IReadOnlyList<string> fallback,
        ref int fallbackIndex)
    {
        if (!string.IsNullOrWhiteSpace(requested))
        {
            var trimmed = requested.Trim();
            if (agents.ContainsKey(trimmed))
                return trimmed;
        }

        var code = fallback[Math.Abs(fallbackIndex) % fallback.Count];
        fallbackIndex++;
        return code;
    }

    private static string Clip(string? value, int maxLength, string fallback = "")
    {
        var text = (value ?? string.Empty).Trim();
        if (text.Length == 0)
            text = fallback;
        return text.Length <= maxLength ? text : text[..maxLength];
    }

    private async Task PersistImportAsync(
        Passage passage,
        Dictionary<string, Emplacement> emplacementsByCode,
        List<StockTheorique> stocks,
        CancellationToken cancellationToken,
        bool wrapTransaction = true)
    {
        if (emplacementsByCode.Count == 0)
            return;

        Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? transaction = null;
        if (wrapTransaction)
            transaction = await _db.Database.BeginTransactionAsync(cancellationToken);

        try
        {
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
            if (transaction is not null)
                await transaction.CommitAsync(cancellationToken);
        }
        finally
        {
            if (transaction is not null)
                await transaction.DisposeAsync();
        }
    }
}
