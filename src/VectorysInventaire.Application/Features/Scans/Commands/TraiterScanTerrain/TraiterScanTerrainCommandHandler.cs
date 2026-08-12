using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Features.Scans.Commands.EnregistrerScan;
using VectorysInventaire.Application.Features.Scans.Commands.FinComptage;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Scans.Commands.TraiterScanTerrain;

public class TraiterScanTerrainCommandHandler : IRequestHandler<TraiterScanTerrainCommand, TraiterScanTerrainResultDto>
{
    private readonly IEmplacementRepository _emplacementRepository;
    private readonly IScanRepository _scanRepository;
    private readonly IStockTheoriqueRepository _stockRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public TraiterScanTerrainCommandHandler(
        IEmplacementRepository emplacementRepository,
        IScanRepository scanRepository,
        IStockTheoriqueRepository stockRepository,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _emplacementRepository = emplacementRepository;
        _scanRepository = scanRepository;
        _stockRepository = stockRepository;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<TraiterScanTerrainResultDto> Handle(
        TraiterScanTerrainCommand request,
        CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var code = request.CodeBarresScan.Trim();
        var agentId = _currentUser.UserId.Value;

        var matchedEmplacement = await _emplacementRepository.GetByAgentIdAndBarcodeAsync(
            agentId, code, cancellationToken);

        if (matchedEmplacement is not null)
        {
            if (matchedEmplacement.Statut == StatutEmplacementEnum.TERMINE)
                throw new InvalidOperationException(
                    $"L'emplacement « {matchedEmplacement.Code} » est déjà terminé.");

            if (request.EmplacementActifId == matchedEmplacement.Id)
            {
                return await BuildEmplacementResultAsync(
                    TraiterScanTerrainTypes.EmplacementDejaActif,
                    matchedEmplacement.Id,
                    message: $"Emplacement « {matchedEmplacement.Code} » déjà actif.",
                    cancellationToken: cancellationToken);
            }

            int? precedentId = null;
            string? precedentCode = null;
            var precedentTermine = false;

            if (request.EmplacementActifId is int previousId && previousId != matchedEmplacement.Id)
            {
                var fin = await _mediator.Send(new FinComptageCommand(previousId), cancellationToken);
                precedentId = fin.EmplacementId;
                var previous = await _emplacementRepository.GetByIdAsync(previousId, cancellationToken);
                precedentCode = previous?.Code;
                precedentTermine = true;
            }

            await ActivateEmplacementAsync(matchedEmplacement.Id, cancellationToken);

            var type = precedentTermine
                ? TraiterScanTerrainTypes.EmplacementBascule
                : TraiterScanTerrainTypes.EmplacementOuvert;

            var message = precedentTermine
                ? $"Emplacement « {precedentCode} » terminé. Comptage ouvert sur « {matchedEmplacement.Code} »."
                : $"Comptage ouvert sur l'emplacement « {matchedEmplacement.Code} ».";

            return await BuildEmplacementResultAsync(
                type,
                matchedEmplacement.Id,
                emplacementPrecedentId: precedentId,
                emplacementPrecedentCode: precedentCode,
                emplacementPrecedentTermine: precedentTermine,
                message: message,
                cancellationToken: cancellationToken);
        }

        if (request.EmplacementActifId is not int actifId)
            throw new InvalidOperationException(
                "Scannez d'abord le code-barres de l'emplacement pour démarrer le comptage.");

        var scanResult = await _mediator.Send(
            new EnregistrerScanCommand(
                code,
                actifId,
                request.DateHeureScan,
                request.IdLocalFlutter),
            cancellationToken);

        var actif = await _emplacementRepository.GetByIdAsync(actifId, cancellationToken)
            ?? throw new KeyNotFoundException("Emplacement actif introuvable.");

        return new TraiterScanTerrainResultDto
        {
            Type = TraiterScanTerrainTypes.UsEnregistre,
            EmplacementActifId = actif.Id,
            EmplacementActifCode = actif.Code,
            EmplacementActifDesignation = actif.Designation,
            QteScannee = scanResult.QteScannee,
            QteAttendue = scanResult.QteAttendue,
            EmplacementStatut = scanResult.EmplacementStatut,
            ScanUs = scanResult,
            Message = $"US « {code} » enregistré ({scanResult.QteScannee} scan(s) sur cet emplacement)."
        };
    }

    private async Task ActivateEmplacementAsync(int emplacementId, CancellationToken cancellationToken)
    {
        var emplacement = await _emplacementRepository.GetByIdForUpdateAsync(emplacementId, cancellationToken)
            ?? throw new KeyNotFoundException("Emplacement introuvable.");

        if (emplacement.AgentAssigneId != _currentUser.UserId)
            throw new UnauthorizedAccessException("Cet emplacement ne vous est pas assigné.");

        if (emplacement.Statut == StatutEmplacementEnum.TERMINE)
            throw new InvalidOperationException("Cet emplacement est déjà terminé.");

        if (emplacement.Statut == StatutEmplacementEnum.EN_ATTENTE)
        {
            emplacement.Statut = StatutEmplacementEnum.EN_COURS;
            await _emplacementRepository.SaveChangesAsync(cancellationToken);
        }
    }

    private async Task<TraiterScanTerrainResultDto> BuildEmplacementResultAsync(
        string type,
        int emplacementId,
        int? emplacementPrecedentId = null,
        string? emplacementPrecedentCode = null,
        bool emplacementPrecedentTermine = false,
        string? message = null,
        CancellationToken cancellationToken = default)
    {
        var emplacement = await _emplacementRepository.GetByIdAsync(emplacementId, cancellationToken)
            ?? throw new KeyNotFoundException("Emplacement introuvable.");
        var qteAttendue = await _stockRepository.GetTotalQteTheoriqueByEmplacementIdAsync(emplacementId, cancellationToken);
        var qteScannee = await _scanRepository.CountByEmplacementIdAsync(emplacementId, cancellationToken);

        return new TraiterScanTerrainResultDto
        {
            Type = type,
            EmplacementActifId = emplacement.Id,
            EmplacementActifCode = emplacement.Code,
            EmplacementActifDesignation = emplacement.Designation,
            EmplacementStatut = emplacement.Statut.ToString(),
            QteScannee = qteScannee,
            QteAttendue = qteAttendue,
            EmplacementPrecedentId = emplacementPrecedentId,
            EmplacementPrecedentCode = emplacementPrecedentCode,
            EmplacementPrecedentTermine = emplacementPrecedentTermine,
            Message = message
        };
    }
}

public static class TraiterScanTerrainTypes
{
    public const string EmplacementOuvert = "EMPLACEMENT_OUVERT";
    public const string EmplacementBascule = "EMPLACEMENT_BASCULE";
    public const string EmplacementDejaActif = "EMPLACEMENT_DEJA_ACTIF";
    public const string UsEnregistre = "US_ENREGISTRE";
}
