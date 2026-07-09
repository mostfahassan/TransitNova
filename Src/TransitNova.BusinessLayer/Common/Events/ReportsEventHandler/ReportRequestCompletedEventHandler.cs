using MediatR;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Interfaces.Repositories.NotificationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.DomainEvents.Events.ReportsEvents;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.BusinessLayer.Common.Events.ReportsEventHandler
{
    public sealed class ReportRequestCompletedEventHandler(
        INotificationCommand notificationRepo,
        IUnitOfWork unitOfWork,
        ILogger<ReportRequestCompletedEventHandler> logger)
        : INotificationHandler<ReportRequestCompletedEvent>
    {
        public async Task Handle(ReportRequestCompletedEvent notification, CancellationToken cancellationToken)
        {
            var userNotification = Notification.Create(
                notification.RequestedBy,
                "Report Generated",
                $"Your requested report {notification.ReportId} has been generated successfully.");

            await notificationRepo.AddNotificationAsync(userNotification, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation(
                "Report request {ReportId} completed and notification sent to User {UserId}.",
                notification.ReportId,
                notification.RequestedBy);
        }
    }
}
