using VectorysInventaire.Domain.Enums;

namespace VectorysInventaire.Domain.Entities;

public class Passage
{
    public int Id { get; set; }
    public int InventaireId { get; set; }
    public TypePassageEnum Type { get; set; }
    public int Numero { get; set; }
    public string Statut { get; set; } = "EN_PREPARATION";
    public DateTime? DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Inventaire? Inventaire { get; set; }
    public ICollection<Emplacement> Emplacements { get; set; } = new List<Emplacement>();
}
