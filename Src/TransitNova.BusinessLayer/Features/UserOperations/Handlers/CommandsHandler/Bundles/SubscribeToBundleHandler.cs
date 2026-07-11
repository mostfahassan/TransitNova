using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Bundles;
using TransitNova.BusinessLayer.Interfaces.Services.BundleService;
using TransitNova.Domain.Contracts.Caching;

namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler.Bundles
{
    public sealed class SubscribeToBundleHandler(
        IBundleSubscription bundleSubscription,
        ILogger<SubscribeToBundleHandler> logger)
        : ICommandHandler<SubscribeToBundleCommand, Result<BundlePaymentInvoiceDto>>
    {
        public async Task<Result<BundlePaymentInvoiceDto>> Handle(SubscribeToBundleCommand request, CancellationToken cancellationToken)
        {
            var result = await bundleSubscription.HandleBundleSubscription(request.UserId, request.BundleId, request.Dto.PaymentMethod, cancellationToken);

            if (result.IsSuccess)
            {
                logger.LogInformation("User {UserId} successfully subscribed to Bundle {BundleId}", request.UserId, request.BundleId);
                CacheInvalidationContext.Set(
                    request,
                    CacheKeys.Users.Profile(request.UserId),
                    CacheKeys.Users.AdminDetails(request.UserId),
                    CacheKeys.Bundles.List,
                    CacheKeys.Bundles.ById(request.BundleId));
            }

            return result;
        }
    }
}
