using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Features.Agents.Queries.GetAgents;

namespace VectorysInventaire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SuperviseurOnly")]
public class AgentsController : ControllerBase
{
    private readonly IMediator _mediator;

    public AgentsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AgentDto>>> GetAll(CancellationToken cancellationToken)
    {
        var list = await _mediator.Send(new GetAgentsQuery(), cancellationToken);
        return Ok(list);
    }
}
