using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Emplacements.Queries.GetEmplacementsByPassage;

public class GetEmplacementsByPassageQueryHandler
    : IRequestHandler<GetEmplacementsByPassageQuery, IReadOnlyList<EmplacementDto>>
{
    private readonly IEmplacementRepository _emplacementRepository;

    public GetEmplacementsByPassageQueryHandler(IEmplacementRepository emplacementRepository)
    {
        _emplacementRepository = emplacementRepository;
    }

    public async Task<IReadOnlyList<EmplacementDto>> Handle(
        GetEmplacementsByPassageQuery request,
        CancellationToken cancellationToken)
    {
        var list = await _emplacementRepository.GetByPassageIdAsync(request.PassageId, cancellationToken);
        return list.Select(e => new EmplacementDto
        {
            Id = e.Id,
            Code = e.Code,
            Designation = e.Designation,
            CodeBarres = e.CodeBarres,
            Statut = e.Statut.ToString(),
            AgentAssigneId = e.AgentAssigneId
        }).ToList();
    }
}
