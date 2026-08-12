using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Kpi.Queries.GetKpiDashboard;

public class GetKpiDashboardQueryHandler : IRequestHandler<GetKpiDashboardQuery, KpiDashboardDto>
{
    private readonly IKpiRepository _kpiRepository;

    public GetKpiDashboardQueryHandler(IKpiRepository kpiRepository) => _kpiRepository = kpiRepository;

    public Task<KpiDashboardDto> Handle(GetKpiDashboardQuery request, CancellationToken cancellationToken) =>
        _kpiRepository.GetDashboardAsync(request.Filter, cancellationToken);
}
