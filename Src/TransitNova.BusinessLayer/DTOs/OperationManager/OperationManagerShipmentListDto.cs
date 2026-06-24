using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.BusinessLayer.DTOs.OperationManager
{

    public class OperationManagerShipmentListDto
    {
        public PagedResult<RetrieveShipmentDto> Shipments { get; set; } = PagedResult<RetrieveShipmentDto>.From([], 0, 1, 20);
    }

}
