using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Kpi.Queries.GetKpiWms;

public class GetKpiWmsQueryHandler : IRequestHandler<GetKpiWmsQuery, KpiWmsDto>
{
    private readonly IKpiRepository _kpiRepository;

    public GetKpiWmsQueryHandler(IKpiRepository kpiRepository) => _kpiRepository = kpiRepository;

    public Task<KpiWmsDto> Handle(GetKpiWmsQuery request, CancellationToken cancellationToken) =>
        _kpiRepository.GetWmsAsync(request.Filter, cancellationToken);
}
