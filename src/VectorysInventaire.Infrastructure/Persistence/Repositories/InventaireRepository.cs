using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Infrastructure.Persistence.Repositories;

public class InventaireRepository : IInventaireRepository
{
    private readonly VectorysDbContext _db;

    public InventaireRepository(VectorysDbContext db) => _db = db;

    public Task<Inventaire?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Inventaires.AsNoTracking().FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public Task<Inventaire?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Inventaires.FirstOrDefaultAsync(i => i.Id == id, cancellationToken);

    public async Task<(IReadOnlyList<Inventaire> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _db.Inventaires.AsNoTracking().OrderByDescending(i => i.CreatedAt);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(cancellationToken);
        return (items, total);
    }

    public Task<int> CountByYearAsync(int year, CancellationToken cancellationToken = default) =>
        _db.Inventaires.CountAsync(i => i.CreatedAt.Year == year, cancellationToken);

    public async Task<int> GetMaxReferenceSequenceForYearAsync(int year, CancellationToken cancellationToken = default)
    {
        var prefix = $"INV-{year}-";
        var references = await _db.Inventaires.AsNoTracking()
            .Where(i => i.Reference.StartsWith(prefix))
            .Select(i => i.Reference)
            .ToListAsync(cancellationToken);

        var max = 0;
        foreach (var reference in references)
        {
            if (reference.Length != prefix.Length + 3)
                continue;
            if (int.TryParse(reference.AsSpan(prefix.Length), out var sequence))
                max = Math.Max(max, sequence);
        }

        return max;
    }

    public async Task AddAsync(Inventaire inventaire, CancellationToken cancellationToken = default) =>
        await _db.Inventaires.AddAsync(inventaire, cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
