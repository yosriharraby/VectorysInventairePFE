namespace VectorysInventaire.Application.DTOs;

public class RecomptageDto
{
    public int Id { get; set; }
    public int InventaireId { get; set; }
    public int PassageC2Id { get; set; }
    public string Mode { get; set; } = string.Empty;
    public string? AssignationMode { get; set; }
    public string? FichierNom { get; set; }
    public string SuperviseurNom { get; set; } = string.Empty;
    public int EmplacementsCrees { get; set; }
    public int EcartsTraites { get; set; }
    public int EcartsRestants { get; set; }
    public string Statut { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public IReadOnlyList<string> EmplacementCodes { get; set; } = Array.Empty<string>();
    public IReadOnlyList<RecomptageAgentDto> Agents { get; set; } = Array.Empty<RecomptageAgentDto>();
}

public class RecomptageAgentDto
{
    public int AgentId { get; set; }
    public string CodePersonnel { get; set; } = string.Empty;
    public string NomComplet { get; set; } = string.Empty;
    public int EmplacementsAssignes { get; set; }
}

public class RecomptagesListDto
{
    public int InventaireId { get; set; }
    public string InventaireReference { get; set; } = string.Empty;
    public int TotalRecomptages { get; set; }
    public IReadOnlyList<RecomptageDto> Items { get; set; } = Array.Empty<RecomptageDto>();
}
