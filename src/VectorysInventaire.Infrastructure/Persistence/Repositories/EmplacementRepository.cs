using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Infrastructure.Persistence.Repositories;

public class EmplacementRepository : IEmplacementRepository
{
    private readonly VectorysDbContext _db;

    public EmplacementRepository(VectorysDbContext db) => _db = db;

    public async Task<IReadOnlyList<Emplacement>> GetByPassageIdAsync(int passageId,
        CancellationToken cancellationToken = default) =>
        await _db.Emplacements.AsNoTracking()
            .Where(e => e.PassageId == passageId)
            .OrderBy(e => e.Code)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Emplacement>> GetByAgentIdAsync(int agentId,
        CancellationToken cancellationToken = default) =>
        await _db.Emplacements.AsNoTracking()
            .Where(e => e.AgentAssigneId == agentId)
            .OrderBy(e => e.Code)
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<Emplacement>> GetByAgentIdWithProgressAsync(int agentId,
        CancellationToken cancellationToken = default) =>
        await _db.Emplacements.AsNoTracking()
            .Include(e => e.Passage)
                .ThenInclude(p => p!.Inventaire)
            .Include(e => e.StocksTheoriques)
            .Include(e => e.Scans)
            .Where(e => e.AgentAssigneId == agentId)
            .OrderBy(e => e.Code)
            .ToListAsync(cancellationToken);

    public async Task AddRangeAsync(IEnumerable<Emplacement> emplacements, CancellationToken cancellationToken = default) =>
        await _db.Emplacements.AddRangeAsync(emplacements, cancellationToken);

    public Task<Emplacement?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Emplacements.AsNoTracking()
            .Include(e => e.Passage)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public Task<Emplacement?> GetByIdForUpdateAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Emplacements.Include(e => e.Passage)
            .FirstOrDefaultAsync(e => e.Id == id, cancellationToken);

    public async Task<IReadOnlyList<Emplacement>> GetByPassageIdWithProgressAsync(int passageId,
        CancellationToken cancellationToken = default) =>
        await _db.Emplacements.AsNoTracking()
            .Include(e => e.AgentAssigne)
            .Include(e => e.StocksTheoriques)
            .Include(e => e.Scans)
            .Where(e => e.PassageId == passageId)
            .OrderBy(e => e.Code)
            .ToListAsync(cancellationToken);

    public Task<Emplacement?> GetByPassageIdAndCodeAsync(int passageId, string code,
        CancellationToken cancellationToken = default) =>
        _db.Emplacements.FirstOrDefaultAsync(e => e.PassageId == passageId && e.Code == code, cancellationToken);

    public async Task<Emplacement?> GetByAgentIdAndBarcodeAsync(int agentId, string code,
        CancellationToken cancellationToken = default)
    {
        var normalized = code.Trim();
        var list = await _db.Emplacements.AsNoTracking()
            .Include(e => e.Passage)
                .ThenInclude(p => p!.Inventaire)
            .Include(e => e.StocksTheoriques)
            .Include(e => e.Scans)
            .Where(e => e.AgentAssigneId == agentId
                        && e.Passage!.Inventaire!.Statut == StatutInventaireEnum.EN_COURS)
            .ToListAsync(cancellationToken);

        return list.FirstOrDefault(e =>
            string.Equals(e.Code, normalized, StringComparison.OrdinalIgnoreCase)
            || string.Equals(e.CodeBarres, normalized, StringComparison.OrdinalIgnoreCase));
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        _db.SaveChangesAsync(cancellationToken);
}
