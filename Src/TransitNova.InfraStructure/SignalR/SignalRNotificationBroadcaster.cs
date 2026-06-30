
using Microsoft.AspNetCore.SignalR;
using TransitNova.BusinessLayer.Interfaces.Services.Notification;
using TransitNova.InfraStructure.SignalR.NotificationHubService;
namespace TransitNova.InfraStructure.SignalR
{
    internal class SignalRNotificationBroadcaster(
    IHubContext<NotificationHub> hubContext) : INotificationBroadcaster
    {
        public async Task SendToUserAsync(Guid userId, string title, string message, CancellationToken cancellationToken)
        {
            await hubContext.Clients.User(userId.ToString())
                  .SendAsync("ReceiveNotification", new { Title = title, Message = message }, cancellationToken);
        }
    }
}
