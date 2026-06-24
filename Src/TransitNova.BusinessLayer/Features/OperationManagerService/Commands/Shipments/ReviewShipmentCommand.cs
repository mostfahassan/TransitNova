using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Shipments
{
    public sealed record ReviewShipmentCommand(Guid ShipmentId, Guid OperationManagerId)
    : ICommand<Result<RetrieveShipmentDto>>;
}
