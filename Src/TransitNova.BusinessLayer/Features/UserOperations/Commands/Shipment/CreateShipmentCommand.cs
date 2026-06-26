using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment
{
    public record CreateShipmentCommand(Guid RequestId , CreateShipmentDto Dto ,Guid AppUserId)
        : IdempotentCommand<Result<RetrieveShipmentDto>>(RequestId), ITransactional;
}