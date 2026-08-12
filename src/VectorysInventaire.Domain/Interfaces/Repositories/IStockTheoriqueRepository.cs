using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Domain.Interfaces.Repositories;

public interface IStockTheoriqueRepository
{
    Task AddRangeAsync(IEnumerable<StockTheorique> lignes, CancellationToken cancellationToken = default);
    Task<int> GetTotalQteTheoriqueByEmplacementIdAsync(int emplacementId,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsCodeUsInEmplacementAsync(int emplacementId, string codeBarres,
        CancellationToken cancellationToken = default);
    Task<int?> GetQteTheoriqueForCodeUsAsync(int emplacementId, string codeUs,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<StockTheorique>> GetByEmplacementIdAsync(int emplacementId,
        CancellationToken cancellationToken = default);
}
