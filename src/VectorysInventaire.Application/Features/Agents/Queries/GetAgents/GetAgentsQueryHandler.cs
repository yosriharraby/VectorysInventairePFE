using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Agents.Queries.GetAgents;

public class GetAgentsQueryHandler : IRequestHandler<GetAgentsQuery, IReadOnlyList<AgentDto>>
{
    private readonly IUtilisateurRepository _utilisateurRepository;

    public GetAgentsQueryHandler(IUtilisateurRepository utilisateurRepository) =>
        _utilisateurRepository = utilisateurRepository;

    public async Task<IReadOnlyList<AgentDto>> Handle(GetAgentsQuery request, CancellationToken cancellationToken)
    {
        var agents = await _utilisateurRepository.GetAgentsAsync(cancellationToken);
        return agents.Select(a => new AgentDto
        {
            Id = a.Id,
            Nom = a.Nom,
            Prenom = a.Prenom,
            CodePersonnel = a.CodePersonnel ?? string.Empty
        }).ToList();
    }
}
