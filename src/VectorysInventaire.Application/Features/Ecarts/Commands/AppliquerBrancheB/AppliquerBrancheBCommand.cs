using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Ecarts.Commands.AppliquerBrancheB;

public record AppliquerBrancheBCommand(int InventaireId) : IRequest<AppliquerBrancheBResultDto>;
