using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Features.Emplacements.Queries.GetEmplacementsByPassage;
using VectorysInventaire.Application.Features.Emplacements.Queries.GetMesEmplacements;

namespace VectorysInventaire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EmplacementsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EmplacementsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    [Authorize(Policy = "SuperviseurOnly")]
    public async Task<ActionResult<IReadOnlyList<EmplacementDto>>> GetByPassage(
        [FromQuery] int passageId,
        CancellationToken cancellationToken)
    {
        var list = await _mediator.Send(new GetEmplacementsByPassageQuery(passageId), cancellationToken);
        return Ok(list);
    }

    [HttpGet("mes-emplacements")]
    [Authorize(Policy = "AgentOnly")]
    public async Task<ActionResult<IReadOnlyList<MesEmplacementDto>>> GetMesEmplacements(
        CancellationToken cancellationToken)
    {
        var list = await _mediator.Send(new GetMesEmplacementsQuery(), cancellationToken);
        return Ok(list);
    }
}
