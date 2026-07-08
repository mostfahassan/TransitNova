using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;

namespace TransitNova.BusinessLayer.Features.Notifications.Commands
{
    public sealed record MarkAllNotificationsAsReadCommand(Guid UserId) : ICommand<BaseResult>;
}