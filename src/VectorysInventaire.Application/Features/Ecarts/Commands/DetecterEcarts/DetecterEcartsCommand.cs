using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Ecarts.Commands.DetecterEcarts;

public record DetecterEcartsCommand(int InventaireId) : IRequest<DetecterEcartsResultDto>;
