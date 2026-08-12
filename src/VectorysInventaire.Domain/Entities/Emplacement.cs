using VectorysInventaire.Domain.Enums;

namespace VectorysInventaire.Domain.Entities;

public class Emplacement
{
    public int Id { get; set; }
    public int PassageId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string CodeBarres { get; set; } = string.Empty;
    public StatutEmplacementEnum Statut { get; set; } = StatutEmplacementEnum.EN_ATTENTE;
    public int? AgentAssigneId { get; set; }

    public Passage? Passage { get; set; }
    public Utilisateur? AgentAssigne { get; set; }
    public ICollection<StockTheorique> StocksTheoriques { get; set; } = new List<StockTheorique>();
    public ICollection<Scan> Scans { get; set; } = new List<Scan>();
}
