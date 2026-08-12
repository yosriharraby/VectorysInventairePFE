using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Kpi.Queries.GetKpiPrecision;

public class GetKpiPrecisionQueryHandler : IRequestHandler<GetKpiPrecisionQuery, KpiPrecisionDto>
{
    private readonly IKpiRepository _kpiRepository;

    public GetKpiPrecisionQueryHandler(IKpiRepository kpiRepository) => _kpiRepository = kpiRepository;

    public Task<KpiPrecisionDto> Handle(GetKpiPrecisionQuery request, CancellationToken cancellationToken) =>
        _kpiRepository.GetPrecisionAsync(request.Filter, cancellationToken);
}
