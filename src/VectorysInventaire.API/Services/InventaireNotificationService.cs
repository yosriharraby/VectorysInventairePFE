using Microsoft.AspNetCore.SignalR;
using VectorysInventaire.API.Hubs;
using VectorysInventaire.Application.DTOs;
using VectorysInventaire.Application.Interfaces.Services;

namespace VectorysInventaire.API.Services;

public class InventaireNotificationService : IInventaireNotificationService
{
    private readonly IHubContext<InventaireHub> _hub;

    public InventaireNotificationService(IHubContext<InventaireHub> hub) => _hub = hub;

    public Task NotifyNouveauScanAsync(NouveauScanEventDto payload, CancellationToken cancellationToken = default) =>
        _hub.Clients.Group(InventaireHub.GroupName(payload.InventaireId))
            .SendAsync("NouveauScan", payload, cancellationToken);

    public Task NotifyFinComptageAsync(FinComptageEventDto payload, CancellationToken cancellationToken = default) =>
        _hub.Clients.Group(InventaireHub.GroupName(payload.InventaireId))
            .SendAsync("FinComptage", payload, cancellationToken);
}
