using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Domain.Interfaces.Repositories;

public interface IInventaireRepository
{
    Task<Inventaire?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Inventaire?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default);
    Task<(IReadOnlyList<Inventaire> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, CancellationToken cancellationToken = default);
    Task<int> CountByYearAsync(int year, CancellationToken cancellationToken = default);
    Task<int> GetMaxReferenceSequenceForYearAsync(int year, CancellationToken cancellationToken = default);
    Task AddAsync(Inventaire inventaire, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
