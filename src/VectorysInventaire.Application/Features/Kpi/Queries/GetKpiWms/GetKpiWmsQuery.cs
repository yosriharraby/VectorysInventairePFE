using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Kpi.Queries.GetKpiWms;

public record GetKpiWmsQuery(KpiFilterDto Filter) : IRequest<KpiWmsDto>;
