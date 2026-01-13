using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace CondoAI.Server.Realtime;

[Authorize]
public class PresenceHub : Hub
{
    public async Task BroadcastPresence(object payload)
    {
        await Clients.All.SendAsync("presence:update", payload);
    }
}
