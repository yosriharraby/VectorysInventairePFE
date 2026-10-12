using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheA;
using VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheB;
using VectorysInventaire.Application.Features.Ecarts.Commands.DetecterEcarts;
using VectorysInventaire.Application.Features.Ecarts.Queries.ExportC1Stock;
using VectorysInventaire.Application.Features.Ecarts.Queries.ExportRecomptageStock;
using VectorysInventaire.Application.Features.Ecarts.Queries.GetEcarts;
using VectorysInventaire.Application.Features.Ecarts.Queries.GetEcartsCount;
using VectorysInventaire.Application.Features.Ecarts.Queries.GetRecomptages;

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

    /// <summary>
    /// Branche B : recomptage via Excel.
    /// mode=GLOBAL → inventaire dès le début (Excel filtré libre).
    /// mode=CIBLE → Excel limité aux emplacements de l'import C1 initial.
    /// </summary>
    [HttpPost("branche-b")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<ActionResult<AppliquerBrancheBResultDto>> AppliquerBrancheB(
        [FromForm] int inventaireId,
        [FromForm] string mode,
        [FromForm] string? assignationMode,
        [FromForm] string? agentCode,
        IFormFile file,
        CancellationToken cancellationToken)
    {
        if (file is null || file.Length == 0)
            return BadRequest(new { message = "Fichier Excel requis." });

        await using var stream = file.OpenReadStream();
        var result = await _mediator.Send(
            new AppliquerBrancheBCommand(
                inventaireId,
                mode,
                stream,
                file.FileName,
                assignationMode,
                agentCode),
            cancellationToken);
        return Ok(result);
    }

    /// <summary>
    /// Liste les recomptages Branche B d'un inventaire (Sprint 6).
    /// </summary>
    [HttpGet("recomptages", Name = "GetRecomptages")]
    [HttpGet("{inventaireId:int}/recomptages", Name = "GetRecomptagesByInventaire")]
    [ProducesResponseType(typeof(RecomptagesListDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<RecomptagesListDto>> GetRecomptages(
        int inventaireId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetRecomptagesQuery(inventaireId), cancellationToken);
        return Ok(result);
    }

    [HttpGet("{inventaireId:int}/export-c1")]
    public async Task<IActionResult> ExportC1Stock(int inventaireId, CancellationToken cancellationToken)
    {
        var (content, fileName) = await _mediator.Send(new ExportC1StockQuery(inventaireId), cancellationToken);
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }

    [HttpGet("recomptages/{recomptageId:int}/export")]
    public async Task<IActionResult> ExportRecomptage(int recomptageId, CancellationToken cancellationToken)
    {
        var (content, fileName) = await _mediator.Send(new ExportRecomptageStockQuery(recomptageId), cancellationToken);
        return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
    }
}
