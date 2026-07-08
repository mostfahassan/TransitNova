using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Notification;

namespace TransitNova.BusinessLayer.Features.Notifications.Queries
{
    public sealed record GetUnreadNotificationsCountQuery(Guid UserId)
        : IQuery<Result<UnreadCountDto>>;
}