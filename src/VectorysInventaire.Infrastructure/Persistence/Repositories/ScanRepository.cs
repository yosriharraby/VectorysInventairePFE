using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Infrastructure.Persistence.Repositories;

public class ScanRepository : IScanRepository
{
    private readonly VectorysDbContext _db;

    public ScanRepository(VectorysDbContext db) => _db = db;

    public async Task AddAsync(Scan scan, CancellationToken cancellationToken = default) =>
        await _db.Scans.AddAsync(scan, cancellationToken);

    public Task<int> CountByEmplacementIdAsync(int emplacementId, CancellationToken cancellationToken = default) =>
        _db.Scans.CountAsync(s => s.EmplacementId == emplacementId, cancellationToken);

    public Task<int> CountByEmplacementIdAndCodeBarresAsync(int emplacementId, string codeBarres,
        CancellationToken cancellationToken = default) =>
        _db.Scans.CountAsync(
            s => s.EmplacementId == emplacementId && s.CodeBarresScan == codeBarres,
            cancellationToken);

    public Task<bool> ExistsByIdLocalFlutterAsync(string idLocalFlutter, CancellationToken cancellationToken = default) =>
        _db.Scans.AnyAsync(s => s.IdLocalFlutter == idLocalFlutter, cancellationToken);

    public Task<DateTime?> GetDernierScanDateAsync(int emplacementId, CancellationToken cancellationToken = default) =>
        _db.Scans.AsNoTracking()
            .Where(s => s.EmplacementId == emplacementId)
            .OrderByDescending(s => s.DateHeureScan)
            .Select(s => (DateTime?)s.DateHeureScan)
            .FirstOrDefaultAsync(cancellationToken);

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
