using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;

namespace TransitNova.BusinessLayer.Features.Shipments.Commands
{
    public record ShipmentFiltrationCommand(ShipmentFilterDto FilterCriteria) 
        : ICommand<Result<PagedResult<RetrieveShipmentDto>>>;
  
}
