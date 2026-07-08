using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace TransitNova.InfraStructure.SignalR.NotificationHubService
{
    [Authorize]
    public sealed class NotificationHub : Hub
    {
    }
}