using MediatR;
using Microsoft.Extensions.Logging;
using TransitNova.Domain.Contracts.DomainEvents.Events.ReportsEvents;

namespace TransitNova.BusinessLayer.Common.Events.ReportsEventHandler
{
    public sealed class ReportGeneratedEventHandler(
        ILogger<ReportGeneratedEventHandler> logger)
        : INotificationHandler<ReportGeneratedEvent>
    {
        public Task Handle(ReportGeneratedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Report {ReportId} generated for {ReportKey}.",
                notification.ReportId,
                notification.ReportKey);

            return Task.CompletedTask;
        }
    }
}
