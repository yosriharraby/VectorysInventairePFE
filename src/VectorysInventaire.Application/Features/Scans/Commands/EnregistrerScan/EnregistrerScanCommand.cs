using MediatR;
using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Features.Scans.Commands.EnregistrerScan;

public record EnregistrerScanCommand(
    string CodeBarresScan,
    int EmplacementId,
    DateTime DateHeureScan,
    string? IdLocalFlutter,
    bool IsOfflineSync = false) : IRequest<ScanEnregistreResultDto>;
