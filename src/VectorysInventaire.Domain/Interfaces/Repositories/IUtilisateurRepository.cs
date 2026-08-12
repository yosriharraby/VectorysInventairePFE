using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Domain.Interfaces.Repositories;

public interface IUtilisateurRepository
{
    Task<Utilisateur?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
    Task<Utilisateur?> GetByCodePersonnelAsync(string codePersonnel, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Utilisateur>> GetAgentsAsync(CancellationToken cancellationToken = default);
}
