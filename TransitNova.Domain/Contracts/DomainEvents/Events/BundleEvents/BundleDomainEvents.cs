namespace TransitNova.Domain.Contracts.DomainEvents.Events.BundleEvents
{
    public sealed record UserSubscribedToBundleDomainEvent(Guid Id, Guid BundleId) : IDomainEvent;
    public sealed record UserUnSubscribedToBundleDomainEvent(Guid Id, Guid BundleId) : IDomainEvent;
    public sealed record BundleUpdatedDomainEvent(Guid Id, decimal BundlePrice) : IDomainEvent;

}
