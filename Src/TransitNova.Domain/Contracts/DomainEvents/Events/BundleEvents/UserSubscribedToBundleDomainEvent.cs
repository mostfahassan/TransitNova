namespace TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents
{
    public sealed record UserSubscribedToBundleDomainEvent(Guid UserProfileId, Guid BundleId) : IDomainEvent;
}