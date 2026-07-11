using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.BundleSubscription;
using TransitNova.BusinessLayer.DTOs.Payment;
namespace TransitNova.BusinessLayer.Features.UserOperations.Commands.Bundles
{
    public sealed record SubscribeToBundleCommand(Guid RequestId, Guid UserId, Guid BundleId, SubscribeToBundleDto Dto)
        : IdempotentCommand<Result<BundlePaymentInvoiceDto>>(RequestId), ICacheInvalidator;
   
}

