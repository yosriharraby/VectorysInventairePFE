using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Agents.Queries.GetAgents;

public record GetAgentsQuery : IRequest<IReadOnlyList<AgentDto>>;
