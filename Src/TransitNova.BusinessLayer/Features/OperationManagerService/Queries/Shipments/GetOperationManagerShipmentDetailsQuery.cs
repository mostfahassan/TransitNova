using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Shipments
{
    public sealed record GetOperationManagerShipmentDetailsQuery(Guid ShipmentId, Guid OperationManagerId)
        : IQuery<Result<RetrieveShipmentDto>>;
}