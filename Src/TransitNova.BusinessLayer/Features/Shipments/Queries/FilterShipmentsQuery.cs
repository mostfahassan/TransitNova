using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;

namespace TransitNova.BusinessLayer.Features.Shipments.Queries
{
    public record FilterShipmentsQuery(ShipmentFilterDto FilterCriteria) 
        : IQuery<Result<PagedResult<RetrieveShipmentDto>>>;
  
}
