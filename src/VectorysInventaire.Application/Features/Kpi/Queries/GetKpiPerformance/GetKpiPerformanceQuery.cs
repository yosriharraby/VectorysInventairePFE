using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Kpi.Queries.GetKpiPerformance;

public record GetKpiPerformanceQuery(KpiFilterDto Filter) : IRequest<KpiPerformanceDto>;
