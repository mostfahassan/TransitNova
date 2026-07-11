using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.Domain.Enums.Payment;

namespace TransitNova.BusinessLayer.Interfaces.Services.BundleService
{
    public interface IBundleSubscription
    {
        Task<Result<BundlePaymentInvoiceDto>> HandleBundleSubscription(Guid userId, Guid bundleId, PaymentMethod paymentMethod, CancellationToken cancellationToken);
    }
}
