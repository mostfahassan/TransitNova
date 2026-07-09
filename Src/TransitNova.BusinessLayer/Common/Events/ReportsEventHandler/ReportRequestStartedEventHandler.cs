using MediatR;
using Microsoft.Extensions.Logging;
using TransitNova.Domain.Contracts.DomainEvents.Events.ReportsEvents;

namespace TransitNova.BusinessLayer.Common.Events.ReportsEventHandler
{
    public sealed class ReportRequestStartedEventHandler(
        ILogger<ReportRequestStartedEventHandler> logger)
        : INotificationHandler<ReportRequestStartedEvent>
    {
        public Task Handle(ReportRequestStartedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation("Report request {ReportId} started.", notification.ReportId);

            return Task.CompletedTask;
        }
    }
}
