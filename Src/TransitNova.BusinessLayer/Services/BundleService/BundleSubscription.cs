using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Interfaces.Repositories.BundleSubscriptionRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.BundleService;
using TransitNova.BusinessLayer.Interfaces.Services.PaymentService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.Services.BundleService
{
    public class BundleSubscriptionPayment(
        IBundleSubscriptionCommandRepository bundleSubscriptionCommandRepository,
        IUserQueryRepository userRepository,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork,
        ILogger<BundleSubscriptionPayment> logger)
        : IBundleSubscription
    {
        public async Task<Result<BundlePaymentInvoiceDto>> HandleBundleSubscription(Guid userId, Guid bundleId, PaymentMethod paymentMethod, CancellationToken cancellationToken)
        {
            var profileId = await userRepository.GetAppUserIdAsync(userId, cancellationToken);
            if (profileId == Guid.Empty)
            {
                logger.LogWarning("User with ID {UserId} not found for bundle subscription", userId);
                return Result<BundlePaymentInvoiceDto>.NotFound(Errors.UserNotFound($"User with ID {userId} not found"));
            }

            var bundle = await bundleSubscriptionCommandRepository.GetBundleForSubscriptionAsync(bundleId, cancellationToken);
            if (bundle is null)
            {
                logger.LogWarning("Bundle with ID {BundleId} not found for subscription", bundleId);
                return Result<BundlePaymentInvoiceDto>.NotFound(Errors.BundleNotFound($"Bundle with ID {bundleId} not found"));
            }

            if (bundle.Subscriptions.Any(subscription => subscription.SubscribedUserId == profileId && subscription.IsActive))
            {
                logger.LogWarning("User {UserId} already has an active subscription to Bundle {BundleId}", userId, bundleId);
                return Result<BundlePaymentInvoiceDto>.Conflict(Errors.Conflict("User is already subscribed to this bundle."));
            }

            var paymentRequest = new CreatePaymentDto
            {
                ReferenceId = bundle.Id,
                PaymentMethod = paymentMethod,
                Currency = Currency.EGP,
                Cost = bundle.BundlePrice
            };

            var paymentResponse = await paymentService.PayForBundle(paymentRequest, cancellationToken);
            if (paymentResponse.IsFailure || paymentResponse.Data is null)
            {
                logger.LogWarning("Bundle payment failed for User {UserId} and Bundle {BundleId}", userId, bundleId);
                return Result<BundlePaymentInvoiceDto>.Failure(paymentResponse.Error ?? Errors.FailedOperation(paymentResponse.Message ?? "Payment operation failed"));
            }

            bundle.Subscribe(profileId);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            var subscription = bundle.Subscriptions.FirstOrDefault(item => item.SubscribedUserId == profileId && item.IsActive);
            var fullName = await userRepository.GetUserFullName(userId, cancellationToken) ?? "Customer account";
            var invoice = paymentResponse.Data;

            var bundleInvoice = new BundlePaymentInvoiceDto
            {
                InvoiceId = $"INV-{invoice.PaymentId.ToString()[..8]}",
                PaymentId = invoice.PaymentId,
                ReferenceId = invoice.ReferenceId,
                BundleId = bundle.Id,
                BundleName = bundle.BundleName,
                FullName = fullName,
                BundlePrice = bundle.BundlePrice,
                Commission = invoice.Commission,
                TotalAmount = invoice.TotalAmount,
                PaymentMethod = invoice.PaymentMethod,
                Status = invoice.Status,
                Currency = Currency.EGP,
                PaidAt = invoice.PaidAt,
                EndDate = subscription?.EndDate,
                SubscribedAt = subscription?.SubscriptionDate,
                Notes = invoice.Notes
            };

            logger.LogInformation("User {UserId} successfully subscribed to Bundle {BundleId}", userId, bundleId);
            return Result<BundlePaymentInvoiceDto>.Success(bundleInvoice);
        }
    }
}
