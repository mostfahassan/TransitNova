using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Notification;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.Notifications
{
    internal class NotificationQueryRepository(AppDbContext context) : INotificationQueryRepository
    {
        public async Task<PagedResult<NotificationDto>> GetNotificationsAsync(Guid userId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            pageNumber = pageNumber <= 0 ? 1 : pageNumber;
            pageSize = pageSize <= 0 ? 20 : pageSize;

            var query = context.Notifications
                .AsNoTracking()
                .Where(notification => notification.UserId == userId)
                .OrderByDescending(notification => notification.CreatedOnUtc);

            var totalCount = await query.CountAsync(cancellationToken);

            var notifications = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(notification => new NotificationDto
                {
                    Id = notification.Id,
                    Title = notification.Title,
                    Message = notification.Message,
                    IsRead = notification.IsRead,
                    CreatedOnUtc = notification.CreatedOnUtc
                })
                .ToListAsync(cancellationToken);

            return PagedResult<NotificationDto>.From(notifications, totalCount, pageNumber, pageSize);
        }

        public Task<int> GetUnreadCountAsync(Guid userId, CancellationToken cancellationToken) =>
            context.Notifications
                .AsNoTracking()
                .CountAsync(notification => notification.UserId == userId && !notification.IsRead, cancellationToken);
    }
}