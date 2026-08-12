using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Infrastructure.Persistence.Repositories;

public class EcartRepository : IEcartRepository
{
    private readonly VectorysDbContext _db;

    public EcartRepository(VectorysDbContext db) => _db = db;

    public async Task<IReadOnlyList<Ecart>> GetByInventaireIdAsync(int inventaireId,
        CancellationToken cancellationToken = default) =>
        await _db.Ecarts.AsNoTracking()
            .Where(e => e.InventaireId == inventaireId)
            .OrderBy(e => e.CodeEmplacement)
            .ThenBy(e => e.CodeUS)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Ecart>> GetByIdsAsync(IEnumerable<int> ids,
        CancellationToken cancellationToken = default)
    {
        var idList = ids.Distinct().ToList();
        return await _db.Ecarts
            .Where(e => idList.Contains(e.Id))
            .ToListAsync(cancellationToken);
    }

    public Task<int> CountByStatutAsync(StatutEcartEnum statut,
        CancellationToken cancellationToken = default) =>
        _db.Ecarts.AsNoTracking().CountAsync(e => e.Statut == statut, cancellationToken);

    public async Task DeleteDetectesByInventaireIdAsync(int inventaireId,
        CancellationToken cancellationToken = default)
    {
        var toRemove = await _db.Ecarts
            .Where(e => e.InventaireId == inventaireId && e.Statut == StatutEcartEnum.DETECTE)
            .ToListAsync(cancellationToken);
        _db.Ecarts.RemoveRange(toRemove);
    }

    public async Task AddRangeAsync(IEnumerable<Ecart> ecarts, CancellationToken cancellationToken = default) =>
        await _db.Ecarts.AddRangeAsync(ecarts, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
