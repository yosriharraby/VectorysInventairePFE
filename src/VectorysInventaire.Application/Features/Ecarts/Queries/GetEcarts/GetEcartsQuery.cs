using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Ecarts.Queries.GetEcarts;

public record GetEcartsQuery(int InventaireId) : IRequest<EcartsListDto>;
