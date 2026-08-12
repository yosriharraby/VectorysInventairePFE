using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Kpi.Queries.GetKpiDashboard;

public record GetKpiDashboardQuery(KpiFilterDto Filter) : IRequest<KpiDashboardDto>;
