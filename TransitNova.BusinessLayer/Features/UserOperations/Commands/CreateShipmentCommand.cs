
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands
{
    public record CreateShipmentCommand(Guid RequestId , CreateShipmentDto Dto ,Guid AppUserId)
        : IdempotantCommand<Result<RetrieveShipmentDto>>(RequestId);
}