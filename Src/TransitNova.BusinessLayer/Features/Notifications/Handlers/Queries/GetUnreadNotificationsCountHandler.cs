using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Notification;
using TransitNova.BusinessLayer.Features.Notifications.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;

namespace TransitNova.BusinessLayer.Features.Notifications.Handlers.Queries
{
    public sealed class GetUnreadNotificationsCountHandler(INotificationQueryRepository notificationQueryRepository)
        : IQueryHandler<GetUnreadNotificationsCountQuery, Result<UnreadCountDto>>
    {
        public async Task<Result<UnreadCountDto>> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
        {
            var unreadCount = await notificationQueryRepository.GetUnreadCountAsync(request.UserId, cancellationToken);

            return Result<UnreadCountDto>.Success(new UnreadCountDto
            {
                Count = unreadCount
            });
        }
    }
}