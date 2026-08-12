namespace VectorysInventaire.Application.DTOs;

public class KpiFilterDto
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int? InventaireId { get; set; }
    public string? Zone { get; set; }
}

public class KpiMetricDto
{
    public string Code { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public string Feasibility { get; set; } = "Full";
    public string? Note { get; set; }
}

public class KpiSeriesPointDto
{
    public string Label { get; set; } = string.Empty;
    public double Value { get; set; }
}

public class KpiOperatorProductivityDto
{
    public int AgentId { get; set; }
    public string Nom { get; set; } = string.Empty;
    public double UnitesParHeure { get; set; }
    public double EmplacementsParHeure { get; set; }
    public int TotalScans { get; set; }
    public int EmplacementsTermines { get; set; }
}

public class KpiClientZoneDto
{
    public string Zone { get; set; } = string.Empty;
    public int InventairesRealises { get; set; }
    public double TauxFiabiliteStock { get; set; }
    public int ValeurEcartsUnites { get; set; }
    public double TempsInventairesHeures { get; set; }
    public string Feasibility { get; set; } = "Partial";
    public string? Note { get; set; }
}

public class KpiFeasibilityNoteDto
{
    public string KpiCode { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Formula { get; set; } = string.Empty;
    public string? MissingData { get; set; }
    public string? ScaleRecommendation { get; set; }
}

public class KpiPrecisionDto
{
    public int NombreEcarts { get; set; }
    public double TauxErreurs { get; set; }
    public int LignesStockControlees { get; set; }
    public IReadOnlyList<KpiSeriesPointDto> EcartsParType { get; set; } = Array.Empty<KpiSeriesPointDto>();
    public IReadOnlyList<KpiSeriesPointDto> EcartsParJour { get; set; } = Array.Empty<KpiSeriesPointDto>();
}

public class KpiPerformanceDto
{
    public int InventairesRealises { get; set; }
    public int InventairesEnCours { get; set; }
    public int EmplacementsTotal { get; set; }
    public int EmplacementsTermines { get; set; }
    public int ScansTotal { get; set; }
    public double TauxAvancement { get; set; }
    public double TempsMoyenRealisationHeures { get; set; }
    public double ReferencesCompteesParHeure { get; set; }
    public IReadOnlyList<KpiOperatorProductivityDto> ProductiviteOperateurs { get; set; } =
        Array.Empty<KpiOperatorProductivityDto>();
    public IReadOnlyList<KpiSeriesPointDto> AvancementParInventaire { get; set; } =
        Array.Empty<KpiSeriesPointDto>();
}

public class KpiQualiteDto
{
    public int CorrectionsStock { get; set; }
    public double TauxValidationPremierComptage { get; set; }
    public int RecomptagesNecessaires { get; set; }
    public double TauxAnomaliesDetectees { get; set; }
    public double TauxConformiteEmplacements { get; set; }
    public IReadOnlyList<KpiSeriesPointDto> Indicateurs { get; set; } = Array.Empty<KpiSeriesPointDto>();
}

public class KpiClientsDto
{
    public string ProxyUsed { get; set; } = "Zone";
    public IReadOnlyList<KpiClientZoneDto> ParZone { get; set; } = Array.Empty<KpiClientZoneDto>();
}

public class KpiWmsDto
{
    public IReadOnlyList<KpiSeriesPointDto> UnitesInventorieesParJour { get; set; } =
        Array.Empty<KpiSeriesPointDto>();
    public IReadOnlyList<KpiSeriesPointDto> EmplacementsInventoriesParJour { get; set; } =
        Array.Empty<KpiSeriesPointDto>();
    public double TauxOccupationEntrepot { get; set; }
    public double DelaiMoyenDetectionCorrectionHeures { get; set; }
    public string OccupationNote { get; set; } = string.Empty;
}

public class KpiDashboardDto
{
    public KpiFilterDto FiltresAppliques { get; set; } = new();
    public DateTime GeneratedAt { get; set; }
    public bool HasData { get; set; }
    public string? EmptyDataMessage { get; set; }
    public KpiPrecisionDto Precision { get; set; } = new();
    public KpiPerformanceDto Performance { get; set; } = new();
    public KpiQualiteDto Qualite { get; set; } = new();
    public KpiClientsDto Clients { get; set; } = new();
    public KpiWmsDto Wms { get; set; } = new();
    public IReadOnlyList<KpiFeasibilityNoteDto> FeasibilityNotes { get; set; } =
        Array.Empty<KpiFeasibilityNoteDto>();
}
