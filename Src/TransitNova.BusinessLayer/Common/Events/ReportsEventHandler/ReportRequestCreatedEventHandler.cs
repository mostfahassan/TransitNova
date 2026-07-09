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
                "Report request {ReportId} created for {ReportType}. ParameterCount: {ParameterCount}",
                notification.ReportId,
                notification.ReportType,
                notification.Parameters.Count);

            await reportGenerationJob.GenerateAsync(notification.ReportId, cancellationToken);
        }
    }
}
