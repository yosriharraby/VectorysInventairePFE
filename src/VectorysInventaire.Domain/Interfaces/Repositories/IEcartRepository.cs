using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;

namespace VectorysInventaire.Domain.Interfaces.Repositories;

public interface IEcartRepository
{
    Task<IReadOnlyList<Ecart>> GetByInventaireIdAsync(int inventaireId,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Ecart>> GetByIdsAsync(IEnumerable<int> ids,
        CancellationToken cancellationToken = default);
    Task<int> CountByStatutAsync(StatutEcartEnum statut,
        CancellationToken cancellationToken = default);
    Task DeleteDetectesByInventaireIdAsync(int inventaireId,
        CancellationToken cancellationToken = default);
    Task AddRangeAsync(IEnumerable<Ecart> ecarts, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
