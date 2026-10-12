using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Ecarts.Queries.GetRecomptages;

public record GetRecomptagesQuery(int InventaireId) : IRequest<RecomptagesListDto>;
