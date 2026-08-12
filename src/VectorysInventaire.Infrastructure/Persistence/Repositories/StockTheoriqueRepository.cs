using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Infrastructure.Persistence.Repositories;

public class StockTheoriqueRepository : IStockTheoriqueRepository
{
    private readonly VectorysDbContext _db;

    public StockTheoriqueRepository(VectorysDbContext db) => _db = db;

    public async Task AddRangeAsync(IEnumerable<StockTheorique> lignes, CancellationToken cancellationToken = default) =>
        await _db.StocksTheoriques.AddRangeAsync(lignes, cancellationToken);

    public Task<int> GetTotalQteTheoriqueByEmplacementIdAsync(int emplacementId,
        CancellationToken cancellationToken = default) =>
        _db.StocksTheoriques.AsNoTracking()
            .Where(s => s.EmplacementId == emplacementId)
            .SumAsync(s => s.QteTheorique, cancellationToken);

    public Task<bool> ExistsCodeUsInEmplacementAsync(int emplacementId, string codeBarres,
        CancellationToken cancellationToken = default) =>
        _db.StocksTheoriques.AsNoTracking()
            .AnyAsync(s => s.EmplacementId == emplacementId && s.CodeUS == codeBarres, cancellationToken);

    public Task<int?> GetQteTheoriqueForCodeUsAsync(int emplacementId, string codeUs,
        CancellationToken cancellationToken = default) =>
        _db.StocksTheoriques.AsNoTracking()
            .Where(s => s.EmplacementId == emplacementId && s.CodeUS == codeUs)
            .Select(s => (int?)s.QteTheorique)
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<IReadOnlyList<StockTheorique>> GetByEmplacementIdAsync(int emplacementId,
        CancellationToken cancellationToken = default) =>
        await _db.StocksTheoriques.AsNoTracking()
            .Where(s => s.EmplacementId == emplacementId)
            .ToListAsync(cancellationToken);
}
