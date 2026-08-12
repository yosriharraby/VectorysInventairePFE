namespace VectorysInventaire.Application.DTOs;

public class AgentDto
{
    public int Id { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string Prenom { get; set; } = string.Empty;
    public string CodePersonnel { get; set; } = string.Empty;
}
