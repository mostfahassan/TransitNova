using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Notification;

namespace TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository
{
    public interface INotificationQueryRepository
    {
        Task<PagedResult<NotificationDto>> GetNotificationsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken);
        Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken);
    }
}