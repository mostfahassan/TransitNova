namespace TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents
{
    public sealed record UserUnsubscribedFromBundleDomainEvent(Guid UserProfileId, Guid BundleId) : IDomainEvent;
}