using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;
using VectorysInventaire.Infrastructure.Persistence;

namespace VectorysInventaire.Infrastructure.Persistence.Repositories;

public class UtilisateurRepository : IUtilisateurRepository
{
    private readonly VectorysDbContext _dbContext;

    public UtilisateurRepository(VectorysDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public Task<Utilisateur?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        return _dbContext.Utilisateurs
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Email == email, cancellationToken);
    }

    public Task<Utilisateur?> GetByCodePersonnelAsync(string codePersonnel, CancellationToken cancellationToken = default)
    {
        return _dbContext.Utilisateurs
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.CodePersonnel == codePersonnel, cancellationToken);
    }

    public async Task<IReadOnlyList<Utilisateur>> GetAgentsAsync(CancellationToken cancellationToken = default) =>
        await _dbContext.Utilisateurs.AsNoTracking()
            .Where(u => u.Role == RoleEnum.AGENT && u.Actif)
            .OrderBy(u => u.Nom)
            .ThenBy(u => u.Prenom)
            .ToListAsync(cancellationToken);
}
