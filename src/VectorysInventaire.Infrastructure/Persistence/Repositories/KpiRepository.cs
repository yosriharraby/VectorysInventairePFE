using Microsoft.EntityFrameworkCore;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Repositories;
using VectorysInventaire.Domain.Enums;

namespace VectorysInventaire.Infrastructure.Persistence.Repositories;

public class KpiRepository : IKpiRepository
{
    private readonly VectorysDbContext _db;

    public KpiRepository(VectorysDbContext db) => _db = db;

    public async Task<KpiDashboardDto> GetDashboardAsync(KpiFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var inventaireIds = await ResolveInventaireIdsAsync(filter, cancellationToken);
        if (inventaireIds.Count == 0)
            return new KpiDashboardDto
            {
                FiltresAppliques = filter,
                GeneratedAt = DateTime.UtcNow,
                HasData = false,
                EmptyDataMessage =
                    "Aucun inventaire ne correspond aux filtres. Créez un inventaire ou élargissez la période.",
                FeasibilityNotes = GetFeasibilityNotes()
            };

        var precision = await GetPrecisionAsync(filter, cancellationToken);
        var performance = await GetPerformanceAsync(filter, cancellationToken);
        var qualite = await GetQualiteAsync(filter, cancellationToken);
        var clients = await GetClientsAsync(filter, cancellationToken);
        var wms = await GetWmsAsync(filter, cancellationToken);

        var hasData = inventaireIds.Count > 0 && (
            precision.NombreEcarts > 0
            || precision.LignesStockControlees > 0
            || performance.InventairesRealises > 0
            || performance.EmplacementsTotal > 0
            || performance.ScansTotal > 0
            || performance.TauxAvancement > 0
            || performance.ProductiviteOperateurs.Count > 0
            || wms.UnitesInventorieesParJour.Count > 0);

        string? emptyMessage = null;
        if (!hasData)
        {
            emptyMessage = filter.InventaireId.HasValue
                ? "Aucune donnée pour cet inventaire. Importez le stock et enregistrez des scans."
                : "Aucune donnée KPI. Créez un inventaire, importez le stock, enregistrez des scans et détectez les écarts.";
        }
        else if (performance.ScansTotal == 0 && performance.EmplacementsTotal > 0)
        {
            emptyMessage = filter.InventaireId.HasValue
                ? "Stock importé — les écarts et la conformité s'afficheront après les premiers scans."
                : "Vue agrégée : certains inventaires n'ont pas encore de scans enregistrés.";
        }

        return new KpiDashboardDto
        {
            FiltresAppliques = filter,
            GeneratedAt = DateTime.UtcNow,
            HasData = hasData,
            EmptyDataMessage = emptyMessage,
            Precision = precision,
            Performance = performance,
            Qualite = qualite,
            Clients = clients,
            Wms = wms,
            FeasibilityNotes = GetFeasibilityNotes()
        };
    }

    public async Task<KpiPrecisionDto> GetPrecisionAsync(KpiFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var inventaireIds = await ResolveInventaireIdsAsync(filter, cancellationToken);
        if (inventaireIds.Count == 0)
            return new KpiPrecisionDto();

        var ecartQuery = _db.Ecarts.AsNoTracking().Where(e => inventaireIds.Contains(e.InventaireId));
        ecartQuery = ApplyEcartDateFilter(ecartQuery, filter);

        var ecarts = await ecartQuery.ToListAsync(cancellationToken);
        var lignesControlees = await CountLignesStockControleesAsync(inventaireIds, cancellationToken);

        var nombreEcarts = ecarts.Count;
        var tauxErreurs = lignesControlees > 0
            ? Math.Round(nombreEcarts * 100.0 / lignesControlees, 2)
            : 0;

        var parType = ecarts
            .GroupBy(e => e.Type)
            .Select(g => new KpiSeriesPointDto
            {
                Label = g.Key.ToString(),
                Value = g.Count()
            })
            .ToList();

        var parJour = ecarts
            .GroupBy(e => e.DetecteAt.Date)
            .OrderBy(g => g.Key)
            .Select(g => new KpiSeriesPointDto
            {
                Label = g.Key.ToString("yyyy-MM-dd"),
                Value = g.Count()
            })
            .ToList();

        return new KpiPrecisionDto
        {
            NombreEcarts = nombreEcarts,
            TauxErreurs = tauxErreurs,
            LignesStockControlees = lignesControlees,
            EcartsParType = parType,
            EcartsParJour = parJour
        };
    }

    public async Task<KpiPerformanceDto> GetPerformanceAsync(KpiFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var inventaireIds = await ResolveInventaireIdsAsync(filter, cancellationToken);
        if (inventaireIds.Count == 0)
            return new KpiPerformanceDto();

        var inventaires = await _db.Inventaires.AsNoTracking()
            .Where(i => inventaireIds.Contains(i.Id))
            .ToListAsync(cancellationToken);

        var inventairesRealises = inventaires.Count(i => i.Statut == StatutInventaireEnum.CLOTURE);

        var c1Passages = await _db.Passages.AsNoTracking()
            .Where(p => inventaireIds.Contains(p.InventaireId) && p.Type == TypePassageEnum.C1)
            .Select(p => new { p.Id, p.InventaireId })
            .ToListAsync(cancellationToken);

        var c1PassageIds = c1Passages.Select(p => p.Id).ToList();

        var emplacementStats = await _db.Emplacements.AsNoTracking()
            .Where(e => c1PassageIds.Contains(e.PassageId))
            .GroupBy(e => e.PassageId)
            .Select(g => new
            {
                PassageId = g.Key,
                Total = g.Count(),
                Termines = g.Count(e => e.Statut == StatutEmplacementEnum.TERMINE)
            })
            .ToListAsync(cancellationToken);

        var passageToInventaire = c1Passages.ToDictionary(p => p.Id, p => p.InventaireId);
        var totalEmplacements = emplacementStats.Sum(s => s.Total);
        var totalTermines = emplacementStats.Sum(s => s.Termines);
        var tauxAvancement = totalEmplacements > 0
            ? Math.Round(totalTermines * 100.0 / totalEmplacements, 2)
            : 0;

        var avancementParInventaire = emplacementStats
            .Where(s => passageToInventaire.ContainsKey(s.PassageId))
            .GroupBy(s => passageToInventaire[s.PassageId])
            .Select(g =>
            {
                var inv = inventaires.First(i => i.Id == g.Key);
                var total = g.Sum(x => x.Total);
                var termines = g.Sum(x => x.Termines);
                return new KpiSeriesPointDto
                {
                    Label = inv.Reference,
                    Value = total > 0 ? Math.Round(termines * 100.0 / total, 2) : 0
                };
            })
            .ToList();

        var clotures = inventaires
            .Where(i => i.Statut == StatutInventaireEnum.CLOTURE && i.DateFin.HasValue)
            .ToList();
        double tempsMoyen;
        if (clotures.Count > 0)
        {
            tempsMoyen = Math.Round(clotures.Average(i => (i.DateFin!.Value - i.DateDebut).TotalHours), 2);
        }
        else
        {
            var emplacementIdsForTime = await _db.Emplacements.AsNoTracking()
                .Where(e => c1PassageIds.Contains(e.PassageId))
                .Select(e => e.Id)
                .ToListAsync(cancellationToken);
            var scanTimes = await _db.Scans.AsNoTracking()
                .Where(s => emplacementIdsForTime.Contains(s.EmplacementId))
                .GroupBy(s => s.EmplacementId)
                .Select(g => new { Min = g.Min(x => x.DateHeureScan), Max = g.Max(x => x.DateHeureScan) })
                .ToListAsync(cancellationToken);
            tempsMoyen = scanTimes.Count > 0
                ? Math.Round(scanTimes.Average(x => (x.Max - x.Min).TotalHours), 2)
                : 0;
        }

        var emplacementIds = await _db.Emplacements.AsNoTracking()
            .Where(e => c1PassageIds.Contains(e.PassageId))
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);

        var scanQuery = _db.Scans.AsNoTracking().Where(s => emplacementIds.Contains(s.EmplacementId));
        scanQuery = ApplyScanDateFilter(scanQuery, filter);
        var scans = await scanQuery
            .Select(s => new { s.AgentId, s.EmplacementId, s.DateHeureScan, s.CodeBarresScan })
            .ToListAsync(cancellationToken);

        var referencesParHeure = 0.0;
        if (scans.Count > 0)
        {
            var minScan = scans.Min(s => s.DateHeureScan);
            var maxScan = scans.Max(s => s.DateHeureScan);
            var hours = Math.Max((maxScan - minScan).TotalHours, 1.0 / 60.0);
            var distinctReferences = scans.Select(s => s.CodeBarresScan).Distinct().Count();
            referencesParHeure = Math.Round(distinctReferences / hours, 2);
        }

        var agentIds = scans.Select(s => s.AgentId).Distinct().ToList();
        var agents = await _db.Utilisateurs.AsNoTracking()
            .Where(u => agentIds.Contains(u.Id))
            .ToDictionaryAsync(u => u.Id, cancellationToken);

        var emplacementsTerminesParAgent = await _db.Emplacements.AsNoTracking()
            .Where(e => c1PassageIds.Contains(e.PassageId)
                        && e.Statut == StatutEmplacementEnum.TERMINE
                        && e.AgentAssigneId.HasValue)
            .GroupBy(e => e.AgentAssigneId!.Value)
            .Select(g => new { AgentId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var productivite = scans
            .GroupBy(s => s.AgentId)
            .Select(g =>
            {
                var min = g.Min(x => x.DateHeureScan);
                var max = g.Max(x => x.DateHeureScan);
                var hours = Math.Max((max - min).TotalHours, 1.0 / 60.0);
                var agent = agents.GetValueOrDefault(g.Key);
                var emplTermines = emplacementsTerminesParAgent.FirstOrDefault(x => x.AgentId == g.Key)?.Count ?? 0;
                return new KpiOperatorProductivityDto
                {
                    AgentId = g.Key,
                    Nom = agent != null ? $"{agent.Prenom} {agent.Nom}".Trim() : $"Agent {g.Key}",
                    TotalScans = g.Count(),
                    EmplacementsTermines = emplTermines,
                    UnitesParHeure = Math.Round(g.Count() / hours, 2),
                    EmplacementsParHeure = Math.Round(emplTermines / hours, 2)
                };
            })
            .OrderByDescending(p => p.UnitesParHeure)
            .ToList();

        return new KpiPerformanceDto
        {
            InventairesRealises = inventairesRealises,
            InventairesEnCours = inventaires.Count(i => i.Statut == StatutInventaireEnum.EN_COURS),
            EmplacementsTotal = totalEmplacements,
            EmplacementsTermines = totalTermines,
            ScansTotal = scans.Count,
            TauxAvancement = tauxAvancement,
            TempsMoyenRealisationHeures = tempsMoyen,
            ReferencesCompteesParHeure = referencesParHeure,
            ProductiviteOperateurs = productivite,
            AvancementParInventaire = avancementParInventaire
        };
    }

    public async Task<KpiQualiteDto> GetQualiteAsync(KpiFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var inventaireIds = await ResolveInventaireIdsAsync(filter, cancellationToken);
        if (inventaireIds.Count == 0)
            return new KpiQualiteDto();

        var ecartQuery = _db.Ecarts.AsNoTracking().Where(e => inventaireIds.Contains(e.InventaireId));
        ecartQuery = ApplyEcartDateFilter(ecartQuery, filter);
        var ecarts = await ecartQuery.ToListAsync(cancellationToken);

        var corrections = ecarts.Count(e => e.Statut == StatutEcartEnum.TRAITE_BRANCHE_A);

        var c1PassageIds = await _db.Passages.AsNoTracking()
            .Where(p => inventaireIds.Contains(p.InventaireId) && p.Type == TypePassageEnum.C1)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        var emplacementsTermines = await _db.Emplacements.AsNoTracking()
            .Where(e => c1PassageIds.Contains(e.PassageId) && e.Statut == StatutEmplacementEnum.TERMINE)
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);

        var emplacementsAvecEcart = ecarts.Select(e => e.EmplacementId).Distinct().ToHashSet();
        var totalTermines = emplacementsTermines.Count;
        var conformes = emplacementsTermines.Count(id => !emplacementsAvecEcart.Contains(id));

        var tauxValidation = totalTermines > 0
            ? Math.Round(conformes * 100.0 / totalTermines, 2)
            : 0;
        var tauxConformite = tauxValidation;

        var recomptages = await _db.Passages.AsNoTracking()
            .CountAsync(p => inventaireIds.Contains(p.InventaireId) && p.Type == TypePassageEnum.C2,
                cancellationToken);

        var lignesControlees = await CountLignesStockControleesAsync(inventaireIds, cancellationToken);
        var tauxAnomalies = lignesControlees > 0
            ? Math.Round(ecarts.Count * 100.0 / lignesControlees, 2)
            : 0;

        var indicateurs = new List<KpiSeriesPointDto>
        {
            new() { Label = "Validation C1 %", Value = tauxValidation },
            new() { Label = "Conformité empl. %", Value = tauxConformite },
            new() { Label = "Anomalies %", Value = tauxAnomalies },
            new() { Label = "Recomptages", Value = recomptages },
            new() { Label = "Corrections", Value = corrections }
        };

        return new KpiQualiteDto
        {
            CorrectionsStock = corrections,
            TauxValidationPremierComptage = tauxValidation,
            RecomptagesNecessaires = recomptages,
            TauxAnomaliesDetectees = tauxAnomalies,
            TauxConformiteEmplacements = tauxConformite,
            Indicateurs = indicateurs
        };
    }

    public async Task<KpiClientsDto> GetClientsAsync(KpiFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var inventaireIds = await ResolveInventaireIdsAsync(filter, cancellationToken);
        if (inventaireIds.Count == 0)
            return new KpiClientsDto();

        var inventaires = await _db.Inventaires.AsNoTracking()
            .Where(i => inventaireIds.Contains(i.Id))
            .ToListAsync(cancellationToken);

        var ecarts = await _db.Ecarts.AsNoTracking()
            .Where(e => inventaireIds.Contains(e.InventaireId))
            .ToListAsync(cancellationToken);

        var lignesParInventaire = await _db.StocksTheoriques.AsNoTracking()
            .Where(st => _db.Emplacements.Any(e =>
                e.Id == st.EmplacementId
                && e.Statut == StatutEmplacementEnum.TERMINE
                && _db.Passages.Any(p =>
                    p.Id == e.PassageId
                    && p.Type == TypePassageEnum.C1
                    && inventaireIds.Contains(p.InventaireId))))
            .Join(_db.Emplacements, st => st.EmplacementId, e => e.Id, (st, e) => new { st, e })
            .Join(_db.Passages, x => x.e.PassageId, p => p.Id, (x, p) => new { x.st, p.InventaireId })
            .GroupBy(x => x.InventaireId)
            .Select(g => new { InventaireId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var parZone = inventaires
            .GroupBy(i => i.Zone)
            .Select(g =>
            {
                var zoneInventaireIds = g.Select(i => i.Id).ToHashSet();
                var zoneEcarts = ecarts.Where(e => zoneInventaireIds.Contains(e.InventaireId)).ToList();
                var zoneLignes = lignesParInventaire
                    .Where(x => zoneInventaireIds.Contains(x.InventaireId))
                    .Sum(x => x.Count);
                var fiabilite = zoneLignes > 0
                    ? Math.Round((zoneLignes - zoneEcarts.Count) * 100.0 / zoneLignes, 2)
                    : 100;

                var tempsHeures = g
                    .Where(i => i.Statut == StatutInventaireEnum.CLOTURE && i.DateFin.HasValue)
                    .Select(i => (i.DateFin!.Value - i.DateDebut).TotalHours)
                    .DefaultIfEmpty(0)
                    .Average();

                return new KpiClientZoneDto
                {
                    Zone = string.IsNullOrWhiteSpace(g.Key) ? "(Sans zone)" : g.Key,
                    InventairesRealises = g.Count(i => i.Statut == StatutInventaireEnum.CLOTURE),
                    TauxFiabiliteStock = fiabilite,
                    ValeurEcartsUnites = zoneEcarts.Sum(e => Math.Abs(e.Delta)),
                    TempsInventairesHeures = Math.Round(tempsHeures, 2),
                    Feasibility = "Partial",
                    Note = "Zone utilisée comme proxy client 3PL. Valeur en unités (pas de prix en base)."
                };
            })
            .OrderByDescending(z => z.InventairesRealises)
            .ToList();

        return new KpiClientsDto
        {
            ProxyUsed = "Zone",
            ParZone = parZone
        };
    }

    public async Task<KpiWmsDto> GetWmsAsync(KpiFilterDto filter,
        CancellationToken cancellationToken = default)
    {
        var inventaireIds = await ResolveInventaireIdsAsync(filter, cancellationToken);
        if (inventaireIds.Count == 0)
            return new KpiWmsDto();

        var c1PassageIds = await _db.Passages.AsNoTracking()
            .Where(p => inventaireIds.Contains(p.InventaireId) && p.Type == TypePassageEnum.C1)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        var emplacementIds = await _db.Emplacements.AsNoTracking()
            .Where(e => c1PassageIds.Contains(e.PassageId))
            .Select(e => e.Id)
            .ToListAsync(cancellationToken);

        var scanQuery = _db.Scans.AsNoTracking().Where(s => emplacementIds.Contains(s.EmplacementId));
        scanQuery = ApplyScanDateFilter(scanQuery, filter);
        var scans = await scanQuery
            .Select(s => new { s.EmplacementId, s.DateHeureScan })
            .ToListAsync(cancellationToken);

        var usParJour = scans
            .GroupBy(s => s.DateHeureScan.Date)
            .OrderBy(g => g.Key)
            .Select(g => new KpiSeriesPointDto
            {
                Label = g.Key.ToString("yyyy-MM-dd"),
                Value = g.Count()
            })
            .ToList();

        var emplacementsTerminesData = await _db.Emplacements.AsNoTracking()
            .Where(e => c1PassageIds.Contains(e.PassageId) && e.Statut == StatutEmplacementEnum.TERMINE)
            .Select(e => new
            {
                DernierScan = e.Scans.Max(s => (DateTime?)s.DateHeureScan)
            })
            .Where(x => x.DernierScan != null)
            .ToListAsync(cancellationToken);

        var emplacementsParJour = emplacementsTerminesData
            .GroupBy(x => x.DernierScan!.Value.Date)
            .OrderBy(g => g.Key)
            .Select(g => new KpiSeriesPointDto
            {
                Label = g.Key.ToString("yyyy-MM-dd"),
                Value = g.Count()
            })
            .ToList();

        var totalEmplacements = await _db.Emplacements.AsNoTracking()
            .CountAsync(e => c1PassageIds.Contains(e.PassageId), cancellationToken);
        var emplacementsActifs = await _db.Emplacements.AsNoTracking()
            .CountAsync(e => c1PassageIds.Contains(e.PassageId) && e.Scans.Any(), cancellationToken);
        var tauxOccupation = totalEmplacements > 0
            ? Math.Round(emplacementsActifs * 100.0 / totalEmplacements, 2)
            : 0;

        var ecartQuery = _db.Ecarts.AsNoTracking()
            .Where(e => inventaireIds.Contains(e.InventaireId)
                        && e.Statut == StatutEcartEnum.TRAITE_BRANCHE_A
                        && e.TraiteAt.HasValue);
        ecartQuery = ApplyEcartDateFilter(ecartQuery, filter);
        var ecartsTraites = await ecartQuery.ToListAsync(cancellationToken);
        var delaiMoyen = ecartsTraites.Count > 0
            ? Math.Round(ecartsTraites.Average(e => (e.TraiteAt!.Value - e.DetecteAt).TotalHours), 2)
            : 0;

        return new KpiWmsDto
        {
            UnitesInventorieesParJour = usParJour,
            EmplacementsInventoriesParJour = emplacementsParJour,
            TauxOccupationEntrepot = tauxOccupation,
            DelaiMoyenDetectionCorrectionHeures = delaiMoyen,
            OccupationNote =
                "Proxy: emplacements avec au moins 1 scan / total emplacements. Capacité entrepôt non modélisée."
        };
    }

    public IReadOnlyList<KpiFeasibilityNoteDto> GetFeasibilityNotes() =>
    [
        new()
        {
            KpiCode = "PRECISION_ECARTS",
            Label = "Nombre d'écarts d'inventaire",
            Status = "Full",
            Formula = "COUNT(Ecart) filtré par inventaire/période",
            ScaleRecommendation = "Index IX_Ecart_InventaireId_DetecteAt recommandé"
        },
        new()
        {
            KpiCode = "PRECISION_TAUX_ERREURS",
            Label = "Taux d'erreurs",
            Status = "Full",
            Formula = "Nb écarts / lignes stock contrôlées (C1, emplacements TERMINE) × 100",
            ScaleRecommendation = "Pré-agréger par inventaire en table de faits nightly"
        },
        new()
        {
            KpiCode = "PERF_INVENTAIRES",
            Label = "Inventaires réalisés",
            Status = "Full",
            Formula = "COUNT(Inventaire WHERE Statut = CLOTURE)",
        },
        new()
        {
            KpiCode = "PERF_AVANCEMENT",
            Label = "Taux d'avancement",
            Status = "Full",
            Formula = "Emplacements TERMINE / total emplacements C1 × 100",
        },
        new()
        {
            KpiCode = "PERF_TEMPS_MOYEN",
            Label = "Temps moyen de réalisation",
            Status = "Partial",
            Formula = "AVG(DateFin - DateDebut) sur inventaires CLOTURE",
            MissingData = "DateFin peut être absente si clôture manuelle non implémentée",
        },
        new()
        {
            KpiCode = "PERF_PRODUCTIVITE",
            Label = "Productivité opérateurs",
            Status = "Full",
            Formula = "Scans/heure et emplacements terminés/heure par agent",
            ScaleRecommendation = "Matérialiser une vue ScanDailySummary"
        },
        new()
        {
            KpiCode = "QUALITE_CORRECTIONS",
            Label = "Corrections de stock",
            Status = "Partial",
            Formula = "COUNT(Ecart WHERE Statut = TRAITE_BRANCHE_A)",
            MissingData = "Branche B et corrections WMS non modélisées",
        },
        new()
        {
            KpiCode = "QUALITE_RECOMPTAGES",
            Label = "Recomptages nécessaires",
            Status = "Partial",
            Formula = "COUNT(Passage Type = C2)",
            MissingData = "Pas de suivi fin C2 vs résolution Branche B",
        },
        new()
        {
            KpiCode = "CLIENT_FIABILITE",
            Label = "KPI par client 3PL",
            Status = "Partial",
            Formula = "Agrégation par Inventaire.Zone (proxy client)",
            MissingData = "Entité Client absente; pas de valeur monétaire des écarts",
        },
        new()
        {
            KpiCode = "WMS_OCCUPATION",
            Label = "Taux d'occupation entrepôt",
            Status = "Partial",
            Formula = "Emplacements scannés / emplacements total (proxy)",
            MissingData = "Capacité maximale entrepôt et emplacements hors inventaire non modélisés",
        },
        new()
        {
            KpiCode = "WMS_DELAI_CORRECTION",
            Label = "Délai détection → correction",
            Status = "Partial",
            Formula = "AVG(TraiteAt - DetecteAt) sur écarts Branche A",
            MissingData = "Écarts non traités exclus; Branche B non incluse",
        }
    ];

    private async Task<List<int>> ResolveInventaireIdsAsync(KpiFilterDto filter,
        CancellationToken cancellationToken)
    {
        var query = _db.Inventaires.AsNoTracking().AsQueryable();

        if (filter.InventaireId.HasValue)
            query = query.Where(i => i.Id == filter.InventaireId.Value);

        if (!string.IsNullOrWhiteSpace(filter.Zone))
            query = query.Where(i => i.Zone == filter.Zone);

        if (filter.DateFrom.HasValue)
            query = query.Where(i => i.DateDebut >= filter.DateFrom.Value);

        if (filter.DateTo.HasValue)
            query = query.Where(i => i.DateDebut <= filter.DateTo.Value);

        return await query.Select(i => i.Id).ToListAsync(cancellationToken);
    }

    private async Task<int> CountLignesStockControleesAsync(IReadOnlyList<int> inventaireIds,
        CancellationToken cancellationToken)
    {
        return await _db.StocksTheoriques.AsNoTracking()
            .Where(st => _db.Emplacements.Any(e =>
                e.Id == st.EmplacementId
                && e.Statut == StatutEmplacementEnum.TERMINE
                && _db.Passages.Any(p =>
                    p.Id == e.PassageId
                    && p.Type == TypePassageEnum.C1
                    && inventaireIds.Contains(p.InventaireId))))
            .CountAsync(cancellationToken);
    }

    private static IQueryable<Domain.Entities.Ecart> ApplyEcartDateFilter(
        IQueryable<Domain.Entities.Ecart> query, KpiFilterDto filter)
    {
        if (filter.DateFrom.HasValue)
            query = query.Where(e => e.DetecteAt >= filter.DateFrom.Value);
        if (filter.DateTo.HasValue)
            query = query.Where(e => e.DetecteAt <= filter.DateTo.Value);
        return query;
    }

    private static IQueryable<Domain.Entities.Scan> ApplyScanDateFilter(
        IQueryable<Domain.Entities.Scan> query, KpiFilterDto filter)
    {
        if (filter.DateFrom.HasValue)
            query = query.Where(s => s.DateHeureScan >= filter.DateFrom.Value);
        if (filter.DateTo.HasValue)
            query = query.Where(s => s.DateHeureScan <= filter.DateTo.Value);
        return query;
    }
}
