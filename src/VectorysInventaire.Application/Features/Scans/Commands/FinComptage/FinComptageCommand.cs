using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Scans.Commands.FinComptage;

public record FinComptageCommand(int EmplacementId) : IRequest<FinComptageResultDto>;
