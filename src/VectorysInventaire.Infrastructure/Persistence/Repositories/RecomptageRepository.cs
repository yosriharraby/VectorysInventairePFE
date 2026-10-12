using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Interfaces.Repositories;
using VectorysInventaire.Infrastructure.Persistence;

namespace VectorysInventaire.Infrastructure.Persistence.Repositories;

public class RecomptageRepository : IRecomptageRepository
{
    private readonly VectorysDbContext _db;

    public RecomptageRepository(VectorysDbContext db) => _db = db;

    public async Task<IReadOnlyList<Recomptage>> GetByInventaireIdAsync(
        int inventaireId,
        CancellationToken cancellationToken = default) =>
        await _db.Recomptages.AsNoTracking()
            .Include(r => r.Superviseur)
            .Include(r => r.Agents).ThenInclude(a => a.Agent)
            .Include(r => r.Emplacements)
            .Where(r => r.InventaireId == inventaireId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);

    public async Task AddAsync(Recomptage recomptage, CancellationToken cancellationToken = default)
    {
        await _db.Recomptages.AddAsync(recomptage, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
