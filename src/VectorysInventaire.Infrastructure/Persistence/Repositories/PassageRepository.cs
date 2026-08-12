using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Infrastructure.Persistence.Repositories;

public class PassageRepository : IPassageRepository
{
    private readonly VectorysDbContext _db;

    public PassageRepository(VectorysDbContext db) => _db = db;

    public Task<Passage?> GetByIdAsync(int id, CancellationToken cancellationToken = default) =>
        _db.Passages.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

    public Task<Passage?> GetByInventaireAndTypeAsync(int inventaireId, TypePassageEnum type,
        CancellationToken cancellationToken = default) =>
        _db.Passages.AsNoTracking()
            .FirstOrDefaultAsync(p => p.InventaireId == inventaireId && p.Type == type, cancellationToken);

    public async Task AddAsync(Passage passage, CancellationToken cancellationToken = default) =>
        await _db.Passages.AddAsync(passage, cancellationToken);
}
