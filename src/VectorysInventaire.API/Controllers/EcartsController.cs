using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheA;
using VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheB;
using VectorysInventaire.Application.Features.Ecarts.Commands.DetecterEcarts;
using VectorysInventaire.Application.Features.Ecarts.Queries.GetEcarts;
using VectorysInventaire.Application.Features.Ecarts.Queries.GetEcartsCount;

namespace VectorysInventaire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SuperviseurOnly")]
public class EcartsController : ControllerBase
{
    private readonly IMediator _mediator;

    public EcartsController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<EcartsListDto>> GetByInventaire(
        [FromQuery] int inventaireId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEcartsQuery(inventaireId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("count")]
    public async Task<ActionResult<EcartsCountDto>> GetCount(CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetEcartsCountQuery(), cancellationToken);
        return Ok(result);
    }

    [HttpPost("detecter/{inventaireId:int}")]
    public async Task<ActionResult<DetecterEcartsResultDto>> Detecter(
        int inventaireId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new DetecterEcartsCommand(inventaireId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("branche-a")]
    public async Task<ActionResult<AppliquerBrancheAResultDto>> AppliquerBrancheA(
        [FromBody] AppliquerBrancheARequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AppliquerBrancheACommand(request.InventaireId, request.EcartIds),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("branche-b")]
    public async Task<ActionResult<AppliquerBrancheBResultDto>> AppliquerBrancheB(
        [FromBody] AppliquerBrancheBRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new AppliquerBrancheBCommand(request.InventaireId),
            cancellationToken);
        return Ok(result);
    }
}
