using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Domain.Interfaces.Repositories;

public interface IScanRepository
{
    Task AddAsync(Scan scan, CancellationToken cancellationToken = default);
    Task<int> CountByEmplacementIdAsync(int emplacementId, CancellationToken cancellationToken = default);
    Task<int> CountByEmplacementIdAndCodeBarresAsync(int emplacementId, string codeBarres,
        CancellationToken cancellationToken = default);
    Task<bool> ExistsByIdLocalFlutterAsync(string idLocalFlutter, CancellationToken cancellationToken = default);
    Task<DateTime?> GetDernierScanDateAsync(int emplacementId, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
