namespace VectorysInventaire.Domain.Entities;

public class StockTheorique
{
    public int Id { get; set; }
    public int EmplacementId { get; set; }
    public string CodeUS { get; set; } = string.Empty;
    public string DesignationUS { get; set; } = string.Empty;
    public int QteTheorique { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Emplacement? Emplacement { get; set; }
}
