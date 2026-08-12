using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;

namespace VectorysInventaire.Domain.Interfaces.Repositories;

public interface IPassageRepository
{
    Task<Passage?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Passage?> GetByInventaireAndTypeAsync(int inventaireId, TypePassageEnum type,
        CancellationToken cancellationToken = default);
    Task AddAsync(Passage passage, CancellationToken cancellationToken = default);
}
