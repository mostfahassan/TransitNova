using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.Payment;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment
{
    public record CreateShipmentCommand(Guid RequestId , CreateShipmentDto Dto ,Guid AppUserId)
        : IdempotentCommand<Result<PaymentInvoiceDto>>(RequestId), ICacheInvalidator;
}

