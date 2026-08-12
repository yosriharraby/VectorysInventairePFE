namespace VectorysInventaire.Application.DTOs;

public class EnregistrerScanRequestDto
{
    public string CodeBarresScan { get; set; } = string.Empty;
    public int EmplacementId { get; set; }
    public DateTime DateHeureScan { get; set; }
    public string? IdLocalFlutter { get; set; }
}

public class TraiterScanTerrainRequestDto
{
    public string CodeBarresScan { get; set; } = string.Empty;
    public int? EmplacementActifId { get; set; }
    public DateTime DateHeureScan { get; set; }
    public string? IdLocalFlutter { get; set; }
}

public class TraiterScanTerrainResultDto
{
    public string Type { get; set; } = string.Empty;
    public int EmplacementActifId { get; set; }
    public string EmplacementActifCode { get; set; } = string.Empty;
    public string EmplacementActifDesignation { get; set; } = string.Empty;
    public string EmplacementStatut { get; set; } = string.Empty;
    public int QteScannee { get; set; }
    public int QteAttendue { get; set; }
    public int? EmplacementPrecedentId { get; set; }
    public string? EmplacementPrecedentCode { get; set; }
    public bool EmplacementPrecedentTermine { get; set; }
    public ScanEnregistreResultDto? ScanUs { get; set; }
    public string? Message { get; set; }
}

public class EnregistrerScansBatchRequestDto
{
    public List<EnregistrerScanRequestDto> Scans { get; set; } = new();
}

public class ScanBatchItemResultDto
{
    public string? IdLocalFlutter { get; set; }
    public bool Success { get; set; }
    public bool IsDuplicate { get; set; }
    public string? ErrorMessage { get; set; }
    public ScanEnregistreResultDto? Result { get; set; }
}

public class EnregistrerScansBatchResultDto
{
    public int TotalRecu { get; set; }
    public int TotalImporte { get; set; }
    public int TotalDoublons { get; set; }
    public int TotalErreurs { get; set; }
    public IReadOnlyList<ScanBatchItemResultDto> Items { get; set; } = Array.Empty<ScanBatchItemResultDto>();
}

public record StockLigneProgressDto(
    string CodeUs,
    string? DesignationUs,
    int QteTheorique,
    int QteScannee);

public class ScanEnregistreResultDto
{
    public int ScanId { get; set; }
    public int EmplacementId { get; set; }
    public int InventaireId { get; set; }
    public string CodeBarresScan { get; set; } = string.Empty;
    public int QteScannee { get; set; }
    public int QteAttendue { get; set; }
    public string EmplacementStatut { get; set; } = string.Empty;
    public bool AutoTermine { get; set; }
    public IReadOnlyList<StockLigneProgressDto> LignesStock { get; set; } = Array.Empty<StockLigneProgressDto>();
}

public class FinComptageResultDto
{
    public int EmplacementId { get; set; }
    public int InventaireId { get; set; }
    public string Statut { get; set; } = string.Empty;
    public int QteScannee { get; set; }
    public int QteAttendue { get; set; }
}

public class NouveauScanEventDto
{
    public int InventaireId { get; set; }
    public int EmplacementId { get; set; }
    public string EmplacementCode { get; set; } = string.Empty;
    public string EmplacementDesignation { get; set; } = string.Empty;
    public int AgentId { get; set; }
    public string AgentNom { get; set; } = string.Empty;
    public string CodeBarresScan { get; set; } = string.Empty;
    public int QteScannee { get; set; }
    public int QteAttendue { get; set; }
    public string EmplacementStatut { get; set; } = string.Empty;
    public DateTime DateHeureScan { get; set; }
}

public class FinComptageEventDto
{
    public int InventaireId { get; set; }
    public int EmplacementId { get; set; }
    public string EmplacementCode { get; set; } = string.Empty;
    public int AgentId { get; set; }
    public string AgentNom { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public int QteScannee { get; set; }
    public int QteAttendue { get; set; }
}

public class SuiviTempsReelDto
{
    public int InventaireId { get; set; }
    public string Intitule { get; set; } = string.Empty;
    public int PassageC1Id { get; set; }
    public IReadOnlyList<AgentSuiviDto> Agents { get; set; } = Array.Empty<AgentSuiviDto>();
}

public class AgentSuiviDto
{
    public int AgentId { get; set; }
    public string Nom { get; set; } = string.Empty;
    public string? CodePersonnel { get; set; }
    public bool EstActif { get; set; }
    public int EmplacementsTermines { get; set; }
    public int EmplacementsTotal { get; set; }
    public IReadOnlyList<EmplacementSuiviDto> Emplacements { get; set; } = Array.Empty<EmplacementSuiviDto>();
}

public class EmplacementSuiviDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string Statut { get; set; } = string.Empty;
    public int QteScannee { get; set; }
    public int QteAttendue { get; set; }
    public DateTime? DernierScan { get; set; }
}
