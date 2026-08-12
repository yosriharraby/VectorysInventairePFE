using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Entities;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Scans.Commands.EnregistrerScan;

public class EnregistrerScanCommandHandler : IRequestHandler<EnregistrerScanCommand, ScanEnregistreResultDto>
{
    private readonly IEmplacementRepository _emplacementRepository;
    private readonly IScanRepository _scanRepository;
    private readonly IStockTheoriqueRepository _stockRepository;
    private readonly IScanCompletionService _scanCompletionService;
    private readonly ICurrentUserService _currentUser;
    private readonly IInventaireNotificationService _notificationService;

    public EnregistrerScanCommandHandler(
        IEmplacementRepository emplacementRepository,
        IScanRepository scanRepository,
        IStockTheoriqueRepository stockRepository,
        IScanCompletionService scanCompletionService,
        ICurrentUserService currentUser,
        IInventaireNotificationService notificationService)
    {
        _emplacementRepository = emplacementRepository;
        _scanRepository = scanRepository;
        _stockRepository = stockRepository;
        _scanCompletionService = scanCompletionService;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    public async Task<ScanEnregistreResultDto> Handle(EnregistrerScanCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var codeUs = request.CodeBarresScan.Trim();

        if (!string.IsNullOrWhiteSpace(request.IdLocalFlutter) &&
            await _scanRepository.ExistsByIdLocalFlutterAsync(request.IdLocalFlutter, cancellationToken))
        {
            return await BuildResultAsync(request.EmplacementId, codeUs, 0, cancellationToken);
        }

        var emplacement = await _emplacementRepository.GetByIdForUpdateAsync(request.EmplacementId, cancellationToken)
            ?? throw new KeyNotFoundException("Emplacement introuvable.");

        if (emplacement.AgentAssigneId != _currentUser.UserId)
            throw new UnauthorizedAccessException("Cet emplacement ne vous est pas assigné.");

        if (emplacement.Statut == StatutEmplacementEnum.TERMINE)
            throw new InvalidOperationException("Cet emplacement est déjà terminé.");

        if (emplacement.Statut == StatutEmplacementEnum.EN_ATTENTE)
            emplacement.Statut = StatutEmplacementEnum.EN_COURS;

        var scan = new Scan
        {
            EmplacementId = request.EmplacementId,
            AgentId = _currentUser.UserId.Value,
            CodeBarresScan = codeUs,
            DateHeureScan = request.DateHeureScan.ToUniversalTime(),
            DateHeureSyncServeur = DateTime.UtcNow,
            EstOffline = request.IsOfflineSync,
            IdLocalFlutter = request.IdLocalFlutter
        };

        await _scanRepository.AddAsync(scan, cancellationToken);
        await _emplacementRepository.SaveChangesAsync(cancellationToken);

        var result = await BuildResultAsync(request.EmplacementId, scan.CodeBarresScan, scan.Id, cancellationToken);
        result.EmplacementStatut = emplacement.Statut.ToString();

        var evt = new NouveauScanEventDto
        {
            InventaireId = result.InventaireId,
            EmplacementId = emplacement.Id,
            EmplacementCode = emplacement.Code,
            EmplacementDesignation = emplacement.Designation,
            AgentId = _currentUser.UserId.Value,
            AgentNom = _currentUser.Nom ?? "Agent",
            CodeBarresScan = scan.CodeBarresScan,
            QteScannee = result.QteScannee,
            QteAttendue = result.QteAttendue,
            EmplacementStatut = emplacement.Statut.ToString(),
            DateHeureScan = scan.DateHeureScan
        };

        await _notificationService.NotifyNouveauScanAsync(evt, cancellationToken);

        return result;
    }

    private async Task<ScanEnregistreResultDto> BuildResultAsync(
        int emplacementId, string codeBarres, int scanId, CancellationToken cancellationToken)
    {
        var emplacement = await _emplacementRepository.GetByIdAsync(emplacementId, cancellationToken)
            ?? throw new KeyNotFoundException("Emplacement introuvable.");
        var inventaireId = emplacement.Passage?.InventaireId
            ?? throw new InvalidOperationException("Passage introuvable pour cet emplacement.");
        var qteAttendue = await _stockRepository.GetTotalQteTheoriqueByEmplacementIdAsync(emplacementId, cancellationToken);
        var qteScannee = await _scanRepository.CountByEmplacementIdAsync(emplacementId, cancellationToken);
        var lignes = await _scanCompletionService.GetProgressAsync(emplacementId, cancellationToken);

        return new ScanEnregistreResultDto
        {
            ScanId = scanId,
            EmplacementId = emplacementId,
            InventaireId = inventaireId,
            CodeBarresScan = codeBarres,
            QteScannee = qteScannee,
            QteAttendue = qteAttendue,
            EmplacementStatut = emplacement.Statut.ToString(),
            AutoTermine = false,
            LignesStock = lignes
        };
    }
}
