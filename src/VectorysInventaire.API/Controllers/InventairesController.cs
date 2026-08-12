using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Features.Inventaires.Commands.CloturerInventaire;
using VectorysInventaire.Application.Features.Inventaires.Commands.CreateInventaire;
using VectorysInventaire.Application.Features.Inventaires.Queries.ExportInventaire;
using VectorysInventaire.Application.Features.Inventaires.Queries.GetInventaires;
using VectorysInventaire.Application.Features.Inventaires.Queries.GetSuiviTempsReel;

namespace VectorysInventaire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SuperviseurOnly")]
public class InventairesController : ControllerBase
{
    private readonly IMediator _mediator;

    public InventairesController(IMediator mediator) => _mediator = mediator;

    [HttpGet]
    public async Task<ActionResult<PagedResultDto<InventaireDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        CancellationToken cancellationToken = default)
    {
        var result = await _mediator.Send(new GetInventairesQuery(page, pageSize), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}/suivi-temps-reel")]
    public async Task<ActionResult<SuiviTempsReelDto>> GetSuiviTempsReel(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetSuiviTempsReelQuery(id), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{id:int}/export")]
    public async Task<IActionResult> Export(int id, CancellationToken cancellationToken)
    {
        var (content, fileName) = await _mediator.Send(new ExportInventaireQuery(id), cancellationToken);
        return File(content,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    [HttpPost]
    public async Task<ActionResult<CreateInventaireResultDto>> Create(
        [FromBody] CreateInventaireRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreateInventaireCommand(request.Intitule, request.Zone, request.DateDebut, request.DateFin),
            cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = result.Inventaire.Id }, result);
    }

    [HttpPost("{id:int}/cloturer")]
    public async Task<ActionResult<CloturerInventaireResultDto>> Cloturer(
        int id,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new CloturerInventaireCommand(id), cancellationToken);
        return Ok(result);
    }
}
