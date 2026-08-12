namespace VectorysInventaire.Application.DTOs;

public class EcartDto
{
    public int Id { get; set; }
    public int InventaireId { get; set; }
    public int EmplacementId { get; set; }
    public string CodeEmplacement { get; set; } = string.Empty;
    public string DesignationEmplacement { get; set; } = string.Empty;
    public string CodeUS { get; set; } = string.Empty;
    public string DesignationUS { get; set; } = string.Empty;
    public int QteTheorique { get; set; }
    public int QteScannee { get; set; }
    public int Delta { get; set; }
    public string Type { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public DateTime DetecteAt { get; set; }
    public DateTime? TraiteAt { get; set; }
}

public class EcartsListDto
{
    public int InventaireId { get; set; }
    public string InventaireReference { get; set; } = string.Empty;
    public int TotalDetectes { get; set; }
    public int TotalTraites { get; set; }
    public int TotalManquants { get; set; }
    public int TotalSurplus { get; set; }
    public IReadOnlyList<EcartDto> Items { get; set; } = Array.Empty<EcartDto>();
}

public class DetecterEcartsResultDto
{
    public int InventaireId { get; set; }
    public int EcartsDetectes { get; set; }
    public IReadOnlyList<EcartDto> Items { get; set; } = Array.Empty<EcartDto>();
}

public class AppliquerBrancheARequestDto
{
    public int InventaireId { get; set; }
    public IReadOnlyList<int> EcartIds { get; set; } = Array.Empty<int>();
}

public class AppliquerBrancheAResultDto
{
    public int InventaireId { get; set; }
    public int PassageC2Id { get; set; }
    public int EcartsTraites { get; set; }
    public int EmplacementsCreesC2 { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class AppliquerBrancheBRequestDto
{
    public int InventaireId { get; set; }
}

public class AppliquerBrancheBResultDto
{
    public int InventaireId { get; set; }
    public int PassageC2Id { get; set; }
    public int EcartsTraites { get; set; }
    public int EmplacementsCreesC2 { get; set; }
    public string Message { get; set; } = string.Empty;
}

public class EcartsCountDto
{
    public int TotalDetectes { get; set; }
}
