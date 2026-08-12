using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Kpi.Queries.GetKpiQualite;

public class GetKpiQualiteQueryHandler : IRequestHandler<GetKpiQualiteQuery, KpiQualiteDto>
{
    private readonly IKpiRepository _kpiRepository;

    public GetKpiQualiteQueryHandler(IKpiRepository kpiRepository) => _kpiRepository = kpiRepository;

    public Task<KpiQualiteDto> Handle(GetKpiQualiteQuery request, CancellationToken cancellationToken) =>
        _kpiRepository.GetQualiteAsync(request.Filter, cancellationToken);
}
