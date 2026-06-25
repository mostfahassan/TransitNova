using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Shipments
{
    public record GetShipmentHistoryQuery(Guid ShipmentId) : IQuery<Result<RetrieveShipmentStatusDto>>;
   
}
