using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.Domain.Entities.MainEntities
{
    public class ShipmentStatus : BaseEntity<int>
    {
        public Guid ShipmentId { get; set; }
        public virtual Shipment Shipment { get; set; } = null!;
        public ShipmentStatuses StatusType { get; set; }
        public Guid? CarrierId { get; set; }
        public virtual Carrier? Carrier { get; set; }
    }
}
