namespace TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents
{
    public sealed record UserSubscribedToBundleDomainEvent(Guid Id, Guid BundleId) : IDomainEvent;

}
