namespace VectorysInventaire.Application.Interfaces.Services;

public interface ICurrentUserService
{
    int? UserId { get; }
    string? Nom { get; }
    string? Role { get; }
    bool IsAuthenticated { get; }
}
