using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Interfaces.Repositories;

public interface IKpiRepository
{
    Task<KpiDashboardDto> GetDashboardAsync(KpiFilterDto filter, CancellationToken cancellationToken = default);
    Task<KpiPrecisionDto> GetPrecisionAsync(KpiFilterDto filter, CancellationToken cancellationToken = default);
    Task<KpiPerformanceDto> GetPerformanceAsync(KpiFilterDto filter, CancellationToken cancellationToken = default);
    Task<KpiQualiteDto> GetQualiteAsync(KpiFilterDto filter, CancellationToken cancellationToken = default);
    Task<KpiClientsDto> GetClientsAsync(KpiFilterDto filter, CancellationToken cancellationToken = default);
    Task<KpiWmsDto> GetWmsAsync(KpiFilterDto filter, CancellationToken cancellationToken = default);
    IReadOnlyList<KpiFeasibilityNoteDto> GetFeasibilityNotes();
}
