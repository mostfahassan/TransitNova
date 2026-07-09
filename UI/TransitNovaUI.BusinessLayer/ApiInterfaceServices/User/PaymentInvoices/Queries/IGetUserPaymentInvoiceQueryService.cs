using TransitNovaUI.BusinessLayer.DTOs.Payment;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.PaymentInvoices.Queries;

public interface IGetUserPaymentInvoiceQueryService
{
    Task<ApiResponse<UiInvoiceDto>> GetUserPaymentInvoiceAsync(Guid paymentId, string bearerToken, CancellationToken cancellationToken = default);
}
