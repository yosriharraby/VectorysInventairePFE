using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Features.Scans.Commands.EnregistrerScan;
using VectorysInventaire.Application.Features.Scans.Commands.EnregistrerScansBatch;
using VectorysInventaire.Application.Features.Scans.Commands.FinComptage;
using VectorysInventaire.Application.Features.Scans.Commands.TraiterScanTerrain;

namespace VectorysInventaire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "AgentOnly")]
public class ScansController : ControllerBase
{
    private readonly IMediator _mediator;

    public ScansController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<ScanEnregistreResultDto>> Enregistrer(
        [FromBody] EnregistrerScanRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new EnregistrerScanCommand(
                request.CodeBarresScan,
                request.EmplacementId,
                request.DateHeureScan,
                request.IdLocalFlutter),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("traiter")]
    public async Task<ActionResult<TraiterScanTerrainResultDto>> TraiterScanTerrain(
        [FromBody] TraiterScanTerrainRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new TraiterScanTerrainCommand(
                request.CodeBarresScan,
                request.EmplacementActifId,
                request.DateHeureScan,
                request.IdLocalFlutter),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("batch")]
    public async Task<ActionResult<EnregistrerScansBatchResultDto>> EnregistrerBatch(
        [FromBody] EnregistrerScansBatchRequestDto request,
        CancellationToken cancellationToken)
    {
        var commands = request.Scans.Select(s => new EnregistrerScanCommand(
            s.CodeBarresScan,
            s.EmplacementId,
            s.DateHeureScan,
            s.IdLocalFlutter,
            IsOfflineSync: true)).ToList();

        var result = await _mediator.Send(new EnregistrerScansBatchCommand(commands), cancellationToken);
        return Ok(result);
    }

    [HttpPut("{emplacementId:int}/fin-comptage")]
    public async Task<ActionResult<FinComptageResultDto>> FinComptage(
        int emplacementId,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new FinComptageCommand(emplacementId), cancellationToken);
        return Ok(result);
    }
}
