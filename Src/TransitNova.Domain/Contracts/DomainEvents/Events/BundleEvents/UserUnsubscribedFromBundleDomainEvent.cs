namespace TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents
{
    public sealed record UserUnsubscribedFromBundleDomainEvent(Guid Id, Guid BundleId) : IDomainEvent;

}
