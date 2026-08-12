namespace VectorysInventaire.Domain.Entities;

public class Scan
{
    public int Id { get; set; }
    public int EmplacementId { get; set; }
    public int AgentId { get; set; }
    public string CodeBarresScan { get; set; } = string.Empty;
    public DateTime DateHeureScan { get; set; }
    public DateTime DateHeureSyncServeur { get; set; } = DateTime.UtcNow;
    public bool EstOffline { get; set; }
    public string? IdLocalFlutter { get; set; }

    public Emplacement? Emplacement { get; set; }
    public Utilisateur? Agent { get; set; }
}
