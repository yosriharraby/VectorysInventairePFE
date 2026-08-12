namespace VectorysInventaire.Application.DTOs;

public class CreateInventaireRequestDto
{
    public string Intitule { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
}

public class InventaireDto
{
    public int Id { get; set; }
    public string Reference { get; set; } = string.Empty;
    public string Intitule { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public DateTime DateDebut { get; set; }
    public DateTime? DateFin { get; set; }
    public string Statut { get; set; } = string.Empty;
    public int SuperviseurId { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateInventaireResultDto
{
    public InventaireDto Inventaire { get; set; } = null!;
    public int PassageC1Id { get; set; }
}

public class CloturerInventaireResultDto
{
    public InventaireDto Inventaire { get; set; } = null!;
    public string Message { get; set; } = string.Empty;
}

public class PagedResultDto<T>
{
    public IReadOnlyList<T> Items { get; set; } = Array.Empty<T>();
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
}

public class CreatePassageRequestDto
{
    public int InventaireId { get; set; }
    public string Type { get; set; } = "C1";
}

public class PassageDto
{
    public int Id { get; set; }
    public int InventaireId { get; set; }
    public string Type { get; set; } = string.Empty;
    public int Numero { get; set; }
    public string Statut { get; set; } = string.Empty;
}

public class ImportStockResultDto
{
    public int EmplacementsCrees { get; set; }
    public int LignesStockImportees { get; set; }
    public Dictionary<string, int> AssignationsParAgent { get; set; } = new();
    public IReadOnlyList<string> AgentsInconnus { get; set; } = Array.Empty<string>();
}

public class EmplacementDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string CodeBarres { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public int? AgentAssigneId { get; set; }
}
