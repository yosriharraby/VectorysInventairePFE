using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Infrastructure.Persistence;

namespace VectorysInventaire.Infrastructure.Services;

public class RecomptageProgressService : IRecomptageProgressService
{
    private readonly VectorysDbContext _db;

    public RecomptageProgressService(VectorysDbContext db) => _db = db;

    public async Task RefreshByEmplacementIdAsync(int emplacementId, CancellationToken cancellationToken = default)
    {
        var recomptageId = await _db.Emplacements.AsNoTracking()
            .Where(e => e.Id == emplacementId)
            .Select(e => e.RecomptageId)
            .FirstOrDefaultAsync(cancellationToken);

        if (recomptageId is null)
            return;

        await RefreshAsync(recomptageId.Value, cancellationToken);
    }

    public async Task RefreshAsync(int recomptageId, CancellationToken cancellationToken = default)
    {
        var recomptage = await _db.Recomptages.FirstOrDefaultAsync(r => r.Id == recomptageId, cancellationToken);
        if (recomptage is null)
            return;

        var emplacements = await _db.Emplacements
            .Include(e => e.StocksTheoriques)
            .Include(e => e.Scans)
            .Where(e => e.RecomptageId == recomptageId)
            .ToListAsync(cancellationToken);

        if (emplacements.Count == 0)
            return;

        var allCorrige = emplacements.All(IsExactMatchAndTermine);
        var codes = emplacements.Select(e => e.Code).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var ecarts = await _db.Ecarts
            .Where(e => e.InventaireId == recomptage.InventaireId
                        && (e.Statut == StatutEcartEnum.DETECTE || e.Statut == StatutEcartEnum.TRAITE_BRANCHE_B)
                        && codes.Contains(e.CodeEmplacement))
            .ToListAsync(cancellationToken);

        var now = DateTime.UtcNow;
        if (allCorrige)
        {
            recomptage.Statut = RecomptageStatuts.Corrige;
            foreach (var ecart in ecarts.Where(e => e.Statut == StatutEcartEnum.DETECTE))
            {
                ecart.Statut = StatutEcartEnum.TRAITE_BRANCHE_B;
                ecart.TraiteAt = now;
            }

            recomptage.EcartsTraites = ecarts.Count(e => e.Statut == StatutEcartEnum.TRAITE_BRANCHE_B);
        }
        else
        {
            recomptage.Statut = RecomptageStatuts.EnAttente;
            foreach (var ecart in ecarts.Where(e => e.Statut == StatutEcartEnum.TRAITE_BRANCHE_B))
            {
                ecart.Statut = StatutEcartEnum.DETECTE;
                ecart.TraiteAt = null;
            }
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    private static bool IsExactMatchAndTermine(Emplacement emp)
    {
        if (emp.Statut != StatutEmplacementEnum.TERMINE)
            return false;

        var scanByCode = emp.Scans
            .GroupBy(s => s.CodeBarresScan.Trim(), StringComparer.OrdinalIgnoreCase)
            .ToDictionary(g => g.Key, g => g.Count(), StringComparer.OrdinalIgnoreCase);

        foreach (var stock in emp.StocksTheoriques)
        {
            scanByCode.TryGetValue(stock.CodeUS.Trim(), out var qte);
            if (qte != stock.QteTheorique)
                return false;
        }

        var stockCodes = emp.StocksTheoriques
            .Select(s => s.CodeUS.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        return scanByCode.Keys.All(stockCodes.Contains);
    }
}
