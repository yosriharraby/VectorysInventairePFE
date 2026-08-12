using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Kpi.Queries.GetKpiFeasibility;

public record GetKpiFeasibilityQuery : IRequest<IReadOnlyList<KpiFeasibilityNoteDto>>;

public class GetKpiFeasibilityQueryHandler : IRequestHandler<GetKpiFeasibilityQuery, IReadOnlyList<KpiFeasibilityNoteDto>>
{
    private readonly IKpiRepository _kpiRepository;

    public GetKpiFeasibilityQueryHandler(IKpiRepository kpiRepository) => _kpiRepository = kpiRepository;

    public Task<IReadOnlyList<KpiFeasibilityNoteDto>> Handle(GetKpiFeasibilityQuery request,
        CancellationToken cancellationToken) =>
        Task.FromResult(_kpiRepository.GetFeasibilityNotes());
}
