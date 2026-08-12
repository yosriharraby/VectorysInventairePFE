using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Features.Passages.Commands.CreatePassage;
using VectorysInventaire.Application.Features.Passages.Commands.ImportStock;

namespace VectorysInventaire.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "SuperviseurOnly")]
public class PassagesController : ControllerBase
{
    private readonly IMediator _mediator;

    public PassagesController(IMediator mediator) => _mediator = mediator;

    [HttpPost]
    public async Task<ActionResult<PassageDto>> Create(
        [FromBody] CreatePassageRequestDto request,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(
            new CreatePassageCommand(request.InventaireId, request.Type),
            cancellationToken);
        return Ok(result);
    }

    [HttpPost("{id:int}/import-stock")]
    [RequestSizeLimit(50 * 1024 * 1024)]
    public async Task<ActionResult<ImportStockResultDto>> ImportStock(
        int id,
        IFormFile file,
        [FromForm] string? codeAgents,
        CancellationToken cancellationToken)
    {
        if (file.Length == 0)
            return BadRequest(new { message = "Fichier vide." });

        IReadOnlyList<string>? defaultCodeAgents = null;
        if (!string.IsNullOrWhiteSpace(codeAgents))
        {
            defaultCodeAgents = codeAgents
                .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .ToList();
        }

        await using var stream = file.OpenReadStream();
        var result = await _mediator.Send(
            new ImportStockCommand(id, stream, defaultCodeAgents),
            cancellationToken);
        return Ok(result);
    }
}
