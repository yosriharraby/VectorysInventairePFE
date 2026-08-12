using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Kpi.Queries.GetKpiClients;

public class GetKpiClientsQueryHandler : IRequestHandler<GetKpiClientsQuery, KpiClientsDto>
{
    private readonly IKpiRepository _kpiRepository;

    public GetKpiClientsQueryHandler(IKpiRepository kpiRepository) => _kpiRepository = kpiRepository;

    public Task<KpiClientsDto> Handle(GetKpiClientsQuery request, CancellationToken cancellationToken) =>
        _kpiRepository.GetClientsAsync(request.Filter, cancellationToken);
}
