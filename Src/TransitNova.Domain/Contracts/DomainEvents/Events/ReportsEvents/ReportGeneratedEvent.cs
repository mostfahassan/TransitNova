namespace TransitNova.Domain.Contracts.DomainEvents.Events.ReportsEvents
{
    public record ReportGeneratedEvent(Guid ReportId, string ReportKey) : IDomainEvent;
}
