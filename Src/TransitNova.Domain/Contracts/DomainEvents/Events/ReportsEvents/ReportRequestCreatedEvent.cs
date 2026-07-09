
using TransitNova.Domain.Enums.Reports;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.ReportsEvents
{
    public record ReportRequestCreatedEvent(Guid ReportId, ReportType ReportType, Dictionary<string, string> Parameters) : IDomainEvent;
    public record ReportRequestStartedEvent(Guid ReportId) : IDomainEvent;
    public record ReportRequestCompletedEvent(Guid ReportId,Guid RequestedBy) : IDomainEvent;
}
