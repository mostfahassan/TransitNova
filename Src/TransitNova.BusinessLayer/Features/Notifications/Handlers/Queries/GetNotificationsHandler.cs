using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Notification;
using TransitNova.BusinessLayer.Features.Notifications.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;

namespace TransitNova.BusinessLayer.Features.Notifications.Handlers.Queries
{
    public sealed class GetNotificationsHandler(INotificationQueryRepository notificationQueryRepository)
        : IQueryHandler<GetNotificationsQuery, Result<PagedResult<NotificationDto>>>
    {
        public async Task<Result<PagedResult<NotificationDto>>> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var notifications = await notificationQueryRepository.GetNotificationsAsync(
                request.UserId,
                request.PageNumber,
                request.PageSize,
                cancellationToken);

            return Result<PagedResult<NotificationDto>>.Success(notifications);
        }
    }
}