using Microsoft.AspNetCore.SignalR;
using WTA.Application.Extensions;

namespace WTA.Application.Abstractions.SignalR;

public static class HubExtensions
{
    public static void ServerToClient(this Hub hub, string method, string message, string toClient, string? fromClient = null)
    {
        hub.Clients.Group(toClient).SendAsync(nameof(ServerToClient).ToSlugify(), method, message, toClient, fromClient);
    }
}
