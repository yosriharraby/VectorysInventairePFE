using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Ecarts.Queries.GetEcartsCount;

public record GetEcartsCountQuery : IRequest<EcartsCountDto>;
