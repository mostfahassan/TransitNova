using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Notification;

namespace TransitNova.BusinessLayer.Features.Notifications.Queries
{
    public sealed record GetNotificationsQuery(Guid UserId, int PageNumber = 1, int PageSize = 20)
        : IQuery<Result<PagedResult<NotificationDto>>>;
}