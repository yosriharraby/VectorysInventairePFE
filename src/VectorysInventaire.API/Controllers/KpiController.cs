using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Features.Kpi.Queries.GetKpiClients;
using VectorysInventaire.Application.Features.Kpi.Queries.GetKpiDashboard;
using VectorysInventaire.Application.Features.Kpi.Queries.GetKpiFeasibility;
using VectorysInventaire.Application.Features.Kpi.Queries.GetKpiPerformance;
using VectorysInventaire.Application.Features.Kpi.Queries.GetKpiPrecision;
using VectorysInventaire.Application.Features.Kpi.Queries.GetKpiQualite;
using VectorysInventaire.Application.Features.Kpi.Queries.GetKpiWms;

namespace VectorysInventaire.API.Controllers;

[ApiController]
[Route("api/kpi")]
[Authorize(Policy = "SuperviseurOnly")]
public class KpiController : ControllerBase
{
    private readonly IMediator _mediator;

    public KpiController(IMediator mediator) => _mediator = mediator;

    [HttpGet("dashboard")]
    public async Task<ActionResult<KpiDashboardDto>> GetDashboard(
        [FromQuery] KpiFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetKpiDashboardQuery(filter), cancellationToken);
        return Ok(result);
    }

    [HttpGet("precision")]
    public async Task<ActionResult<KpiPrecisionDto>> GetPrecision(
        [FromQuery] KpiFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetKpiPrecisionQuery(filter), cancellationToken);
        return Ok(result);
    }

    [HttpGet("performance")]
    public async Task<ActionResult<KpiPerformanceDto>> GetPerformance(
        [FromQuery] KpiFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetKpiPerformanceQuery(filter), cancellationToken);
        return Ok(result);
    }

    [HttpGet("qualite")]
    public async Task<ActionResult<KpiQualiteDto>> GetQualite(
        [FromQuery] KpiFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetKpiQualiteQuery(filter), cancellationToken);
        return Ok(result);
    }

    [HttpGet("clients")]
    public async Task<ActionResult<KpiClientsDto>> GetClients(
        [FromQuery] KpiFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetKpiClientsQuery(filter), cancellationToken);
        return Ok(result);
    }

    [HttpGet("wms")]
    public async Task<ActionResult<KpiWmsDto>> GetWms(
        [FromQuery] KpiFilterDto filter,
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetKpiWmsQuery(filter), cancellationToken);
        return Ok(result);
    }

    [HttpGet("feasibility")]
    public async Task<ActionResult<IReadOnlyList<KpiFeasibilityNoteDto>>> GetFeasibility(
        CancellationToken cancellationToken)
    {
        var result = await _mediator.Send(new GetKpiFeasibilityQuery(), cancellationToken);
        return Ok(result);
    }
}
