using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Kpi.Queries.GetKpiQualite;

public record GetKpiQualiteQuery(KpiFilterDto Filter) : IRequest<KpiQualiteDto>;
