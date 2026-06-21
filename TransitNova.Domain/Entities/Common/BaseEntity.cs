using TransitNova.Domain.Contracts.DomainEvents;

namespace TransitNova.Domain.Entities.Common
{
    public class BaseEntity<Tkey>
    {
        public Tkey Id { get; init; } = default!;
        public DateTime CreatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? UpdatedBy { get; set; }
        public bool CurrentState { get; set; }


    }
}
