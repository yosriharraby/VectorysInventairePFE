using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Inventaires.Commands.CloturerInventaire;

public record CloturerInventaireCommand(int InventaireId) : IRequest<CloturerInventaireResultDto>;
