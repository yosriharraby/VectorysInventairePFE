using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Inventaires.Queries.GetSuiviTempsReel;

public record GetSuiviTempsReelQuery(int InventaireId) : IRequest<SuiviTempsReelDto>;
