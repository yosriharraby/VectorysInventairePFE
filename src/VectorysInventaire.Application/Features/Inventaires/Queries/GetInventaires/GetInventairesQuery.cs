using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Inventaires.Queries.GetInventaires;

public record GetInventairesQuery(int Page = 1, int PageSize = 20) : IRequest<PagedResultDto<InventaireDto>>;
