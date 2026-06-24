using TransitNova.Domain.Contracts.DomainEvents;

namespace TransitNova.Domain.Entities.Common
{
    public class BaseEntity<Tkey>
    {
        public Tkey Id { get; init; } = default!;
        public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
        public string? CreatedBy { get; protected set; }
        public DateTime? UpdatedAt { get; protected set; }
        public string? UpdatedBy { get; protected set; }
        public bool CurrentState { get;  set; }
    }
}
