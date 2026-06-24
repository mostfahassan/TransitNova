namespace TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents
{
    public sealed record BundleUpdatedDomainEvent(Guid Id, decimal BundlePrice) : IDomainEvent;

}
