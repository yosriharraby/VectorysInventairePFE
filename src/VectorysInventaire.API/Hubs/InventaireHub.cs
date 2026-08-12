using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace VectorysInventaire.API.Hubs;

[Authorize(Policy = "SuperviseurOnly")]
public class InventaireHub : Hub
{
    public async Task JoinInventaireGroup(int inventaireId)
    {
        await Groups.AddToGroupAsync(Context.ConnectionId, GroupName(inventaireId));
    }

    public async Task LeaveInventaireGroup(int inventaireId)
    {
        await Groups.RemoveFromGroupAsync(Context.ConnectionId, GroupName(inventaireId));
    }

    public static string GroupName(int inventaireId) => $"inventaire_{inventaireId}";
}
