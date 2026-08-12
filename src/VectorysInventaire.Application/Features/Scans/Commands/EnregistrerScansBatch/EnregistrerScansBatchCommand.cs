using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Features.Scans.Commands.EnregistrerScan;

namespace VectorysInventaire.Application.Features.Scans.Commands.EnregistrerScansBatch;

public record EnregistrerScansBatchCommand(IReadOnlyList<EnregistrerScanCommand> Scans)
    : IRequest<EnregistrerScansBatchResultDto>;
