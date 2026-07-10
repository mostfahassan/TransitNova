using MediatR;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Interfaces.Services.Reports;
using TransitNova.Domain.Contracts.DomainEvents.Events.ReportsEvents;

namespace TransitNova.BusinessLayer.Common.Events.ReportsEventHandler
{
    public sealed class ReportRequestCreatedEventHandler(
        IReportGenerationJob reportGenerationJob,
        ILogger<ReportRequestCreatedEventHandler> logger)
        : INotificationHandler<ReportRequestCreatedEvent>
    {
        public async Task Handle(ReportRequestCreatedEvent notification, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Report request {ReportId} created for {ReportKey}.",
                notification.ReportId,
                notification.ReportKey);

            await reportGenerationJob.DelegateToBackgroundAsync(notification.ReportId, cancellationToken);
        }
    }
}
