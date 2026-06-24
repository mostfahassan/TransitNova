
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Admin
{

    public sealed class TopOperationManagerDto
    {
        public Guid OperationManagerId { get; set; }

        public string FullName { get; set; } = string.Empty;

        public int ApprovedShipments { get; set; }
    }

}
