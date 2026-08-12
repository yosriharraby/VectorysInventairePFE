using VectorysInventaire.Domain.Enums;

namespace VectorysInventaire.Domain.Entities;

public class Inventaire
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public StatutInventaireEnum Statut { get; set; } = StatutInventaireEnum.EN_PREPARATION;
    public int SuperviseurId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Utilisateur? Superviseur { get; set; }
    public ICollection<Passage> Passages { get; set; } = new List<Passage>();
}
