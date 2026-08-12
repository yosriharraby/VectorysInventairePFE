using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Passages.Commands.CreatePassage;

public record CreatePassageCommand(int InventaireId, string Type) : IRequest<PassageDto>;
