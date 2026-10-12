namespace VectorysInventaire.Domain.Entities;

/// <summary>
/// Trace un recomptage Branche B lié à l'inventaire parent (C1).
/// </summary>
public class Recomptage
{
    public int Id { get; set; }
    public int InventaireId { get; set; }
    public int PassageC2Id { get; set; }
    public string Mode { get; set; } = "GLOBAL";
    public string? AssignationMode { get; set; }
    public string? FichierNom { get; set; }
    public int SuperviseurId { get; set; }
    public int EmplacementsCrees { get; set; }
    public int EcartsTraites { get; set; }
    public string Statut { get; set; } = RecomptageStatuts.EnAttente;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Inventaire? Inventaire { get; set; }
    public Passage? PassageC2 { get; set; }
    public Utilisateur? Superviseur { get; set; }
    public ICollection<RecomptageAgent> Agents { get; set; } = new List<RecomptageAgent>();
    public ICollection<Emplacement> Emplacements { get; set; } = new List<Emplacement>();
}

public static class RecomptageStatuts
{
    public const string EnAttente = "EN_ATTENTE";
    public const string Corrige = "CORRIGE";

    public static string Libelle(string mode, string? fichierNom)
    {
        var type = string.Equals(mode, "CIBLE", StringComparison.OrdinalIgnoreCase)
            ? "Recomptage ciblé"
            : "Recomptage intégral";
        return string.IsNullOrWhiteSpace(fichierNom) ? type : $"{type} — {fichierNom}";
    }
}

public class RecomptageAgent
{
    public int Id { get; set; }
    public int RecomptageId { get; set; }
    public int AgentId { get; set; }
    public int EmplacementsAssignes { get; set; }

    public Recomptage? Recomptage { get; set; }
    public Utilisateur? Agent { get; set; }
}
