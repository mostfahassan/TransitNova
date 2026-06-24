namespace TransitNova.Domain.Contracts.DomainEvents
{
    public interface IAggregateRoot
    {
        public IReadOnlyCollection<IDomainEvent> GetDomainEvents();
        public void ReleaseDomainEvent(IDomainEvent domainEvent);
        public void RaiseDomainEvent(IDomainEvent domainEvent);
        public void ClearDomainEvents();
    }


}
