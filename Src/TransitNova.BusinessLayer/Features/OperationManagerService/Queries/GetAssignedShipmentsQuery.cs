
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries
{
    public sealed record GetAssignedShipmentsQuery(ShipmentFilterDto Dto) : IQuery<Result<PagedResult<RetrieveShipmentDto>>>;
   
}

