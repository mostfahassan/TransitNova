using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Notifications.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;

namespace TransitNova.BusinessLayer.Features.Notifications.Handlers.Commands
{
    public sealed class MarkAllNotificationsAsReadHandler(INotificationCommand notificationCommand)
        : ICommandHandler<MarkAllNotificationsAsReadCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            await notificationCommand.MarkAllAsReadAsync(request.UserId, cancellationToken);
            return BaseResult.Success();
        }
    }
}