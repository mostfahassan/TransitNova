using TransitNova.Domain.Enums.Reports;

namespace TransitNova.Domain.Contracts.DomainEvents.Events.ReportsEvents
{
    public record ReportGeneratedEvent(Guid reportId, ReportType reportType, Dictionary<string, string> parameters) : IDomainEvent;
   
}
