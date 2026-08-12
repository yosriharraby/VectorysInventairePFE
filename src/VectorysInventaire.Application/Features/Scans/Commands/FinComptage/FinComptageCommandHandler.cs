using MediatR;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Services;
using VectorysInventaire.Domain.Enums;
using VectorysInventaire.Domain.Interfaces.Repositories;

namespace VectorysInventaire.Application.Features.Scans.Commands.FinComptage;

public class FinComptageCommandHandler : IRequestHandler<FinComptageCommand, FinComptageResultDto>
{
    private readonly IEmplacementRepository _emplacementRepository;
    private readonly IScanRepository _scanRepository;
    private readonly IStockTheoriqueRepository _stockRepository;
    private readonly ICurrentUserService _currentUser;
    private readonly IInventaireNotificationService _notificationService;

    public FinComptageCommandHandler(
        IEmplacementRepository emplacementRepository,
        IScanRepository scanRepository,
        IStockTheoriqueRepository stockRepository,
        ICurrentUserService currentUser,
        IInventaireNotificationService notificationService)
    {
        _emplacementRepository = emplacementRepository;
        _scanRepository = scanRepository;
        _stockRepository = stockRepository;
        _currentUser = currentUser;
        _notificationService = notificationService;
    }

    public async Task<FinComptageResultDto> Handle(FinComptageCommand request, CancellationToken cancellationToken)
    {
        if (_currentUser.UserId is null)
            throw new UnauthorizedAccessException("Utilisateur non authentifié.");

        var emplacement = await _emplacementRepository.GetByIdForUpdateAsync(request.EmplacementId, cancellationToken)
            ?? throw new KeyNotFoundException("Emplacement introuvable.");

        if (emplacement.AgentAssigneId != _currentUser.UserId)
            throw new UnauthorizedAccessException("Cet emplacement ne vous est pas assigné.");

        if (emplacement.Statut == StatutEmplacementEnum.TERMINE)
            throw new InvalidOperationException("Cet emplacement est déjà terminé.");

        emplacement.Statut = StatutEmplacementEnum.TERMINE;
        await _emplacementRepository.SaveChangesAsync(cancellationToken);

        var qteAttendue = await _stockRepository.GetTotalQteTheoriqueByEmplacementIdAsync(
            request.EmplacementId, cancellationToken);
        var qteScannee = await _scanRepository.CountByEmplacementIdAsync(request.EmplacementId, cancellationToken);
        var inventaireId = emplacement.Passage?.InventaireId
            ?? throw new InvalidOperationException("Passage introuvable pour cet emplacement.");

        var result = new FinComptageResultDto
        {
            EmplacementId = emplacement.Id,
            InventaireId = inventaireId,
            Statut = emplacement.Statut.ToString(),
            QteScannee = qteScannee,
            QteAttendue = qteAttendue
        };

        await _notificationService.NotifyFinComptageAsync(new FinComptageEventDto
        {
            InventaireId = inventaireId,
            EmplacementId = emplacement.Id,
            EmplacementCode = emplacement.Code,
            AgentId = _currentUser.UserId.Value,
            AgentNom = _currentUser.Nom ?? "Agent",
            Statut = emplacement.Statut.ToString(),
            QteScannee = qteScannee,
            QteAttendue = qteAttendue
        }, cancellationToken);

        return result;
    }
}
