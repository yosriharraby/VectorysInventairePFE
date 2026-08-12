using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Kpi.Queries.GetKpiPrecision;

public record GetKpiPrecisionQuery(KpiFilterDto Filter) : IRequest<KpiPrecisionDto>;
