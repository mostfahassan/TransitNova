
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Admin
{

    public class ShipmentStatusStatDto
    {
        public ShipmentStatuses Status { get; set; }
        public int Count { get; set; }
    }

}
