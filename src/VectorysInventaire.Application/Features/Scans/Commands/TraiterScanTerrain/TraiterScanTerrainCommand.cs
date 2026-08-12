using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Scans.Commands.TraiterScanTerrain;

public record TraiterScanTerrainCommand(
    string CodeBarresScan,
    int? EmplacementActifId,
    DateTime DateHeureScan,
    string? IdLocalFlutter) : IRequest<TraiterScanTerrainResultDto>;
