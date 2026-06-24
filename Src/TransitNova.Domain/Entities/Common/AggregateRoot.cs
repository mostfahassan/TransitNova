using TransitNova.Domain.Contracts.DomainEvents;
namespace TransitNova.Domain.Entities.Common
{
    public abstract class AggregateRoot<TKey> : BaseEntity<TKey>, IAggregateRoot
    {
        private readonly List<IDomainEvent> _domainEvents = new();
        public IReadOnlyCollection<IDomainEvent> GetDomainEvents () => _domainEvents;
        public void RaiseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Add(domainEvent);
        }
        public void ClearDomainEvents()
        {
            _domainEvents.Clear();
        }
        public void ReleaseDomainEvent(IDomainEvent domainEvent)
        {
            _domainEvents.Remove(domainEvent);
        }
    }
}
