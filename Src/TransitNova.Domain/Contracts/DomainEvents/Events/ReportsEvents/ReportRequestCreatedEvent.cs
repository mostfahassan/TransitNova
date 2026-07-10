namespace TransitNova.Domain.Contracts.DomainEvents.Events.ReportsEvents
{
    public record ReportRequestCreatedEvent(Guid ReportId, string ReportKey) : IDomainEvent;
    public record ReportRequestStartedEvent(Guid ReportId) : IDomainEvent;
    public record ReportRequestCompletedEvent(Guid ReportId, Guid RequestedBy) : IDomainEvent;
}
