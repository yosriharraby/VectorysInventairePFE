using VectorysInventaire.Domain.Entities;

namespace VectorysInventaire.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(Utilisateur user);
}
