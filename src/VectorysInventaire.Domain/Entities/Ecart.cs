using VectorysInventaire.Domain.Enums;

namespace VectorysInventaire.Domain.Entities;

public class Ecart
{
    public int Id { get; set; }
    public int InventaireId { get; set; }
    public int PassageId { get; set; }
    public int EmplacementId { get; set; }
    public string CodeEmplacement { get; set; } = string.Empty;
    public string DesignationEmplacement { get; set; } = string.Empty;
    public string CodeUS { get; set; } = string.Empty;
    public string DesignationUS { get; set; } = string.Empty;
    public int QteTheorique { get; set; }
    public int QteScannee { get; set; }
    public int Delta { get; set; }
    public TypeEcartEnum Type { get; set; }
    public StatutEcartEnum Statut { get; set; } = StatutEcartEnum.DETECTE;
    public DateTime DetecteAt { get; set; } = DateTime.UtcNow;
    public DateTime? TraiteAt { get; set; }

    public Inventaire? Inventaire { get; set; }
    public Passage? Passage { get; set; }
    public Emplacement? Emplacement { get; set; }
}
