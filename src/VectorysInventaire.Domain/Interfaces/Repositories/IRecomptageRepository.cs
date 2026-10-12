using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Domain.Interfaces.Repositories;

public interface IRecomptageRepository
{
    Task<IReadOnlyList<Recomptage>> GetByInventaireIdAsync(int inventaireId, CancellationToken cancellationToken = default);
    Task AddAsync(Recomptage recomptage, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
