using VectorysInventaire.Application.DTOs;

namespace VectorysInventaire.Application.Interfaces.Services;

public interface IInventaireNotificationService
{
    Task NotifyNouveauScanAsync(NouveauScanEventDto payload, CancellationToken cancellationToken = default);
    Task NotifyFinComptageAsync(FinComptageEventDto payload, CancellationToken cancellationToken = default);
}
