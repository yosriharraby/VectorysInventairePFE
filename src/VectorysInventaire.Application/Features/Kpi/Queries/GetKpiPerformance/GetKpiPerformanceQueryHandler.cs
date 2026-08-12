using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Kpi.Queries.GetKpiPerformance;

public class GetKpiPerformanceQueryHandler : IRequestHandler<GetKpiPerformanceQuery, KpiPerformanceDto>
{
    private readonly IKpiRepository _kpiRepository;

    public GetKpiPerformanceQueryHandler(IKpiRepository kpiRepository) => _kpiRepository = kpiRepository;

    public Task<KpiPerformanceDto> Handle(GetKpiPerformanceQuery request, CancellationToken cancellationToken) =>
        _kpiRepository.GetPerformanceAsync(request.Filter, cancellationToken);
}
