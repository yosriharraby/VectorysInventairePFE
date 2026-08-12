using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Domain.Interfaces.Repositories;

public interface IEmplacementRepository
{
    Task<IReadOnlyList<Emplacement>> GetByPassageIdAsync(int passageId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Emplacement>> GetByAgentIdAsync(int agentId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Emplacement>> GetByAgentIdWithProgressAsync(int agentId,
        CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Emplacement> emplacements, CancellationToken cancellationToken = default);
    Task<Emplacement?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Emplacement?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Emplacement>> GetByPassageIdWithProgressAsync(int passageId,
        CancellationToken cancellationToken = default);
    Task<Emplacement?> GetByPassageIdAndCodeAsync(int passageId, string code,
        CancellationToken cancellationToken = default);
    Task<Emplacement?> GetByAgentIdAndBarcodeAsync(int agentId, string code,
        CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
