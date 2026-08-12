using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheA;

public record AppliquerBrancheACommand(int InventaireId, IReadOnlyList<int> EcartIds)
    : IRequest<AppliquerBrancheAResultDto>;
