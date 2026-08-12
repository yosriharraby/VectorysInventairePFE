using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Kpi.Queries.GetKpiClients;

public record GetKpiClientsQuery(KpiFilterDto Filter) : IRequest<KpiClientsDto>;
