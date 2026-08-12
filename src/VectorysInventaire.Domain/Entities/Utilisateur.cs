using VectorysInventaire.Domain.Enums;

namespace VectorysInventaire.Domain.Entities;

public class Utilisateur
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string? CodePersonnel { get; set; }
    public string? Email { get; set; }
    public string PasswordHash { get; set; } = string.Empty;
    public RoleEnum Role { get; set; }
    public bool Actif { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
