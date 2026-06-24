
using Microsoft.AspNetCore.SignalR;
using TransitNova.BusinessLayer.Interfaces.Services.Notification;
using TransitNova.InfraStructure.Common.NotificationService.NotificationHubService;
namespace TransitNova.InfraStructure.Common.NotificationService
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
