using TransitNovaUI.BusinessLayer.DTOs.Payment;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.PaymentInvoices.Queries;

public interface IGetUserPaymentInvoicesQueryService
{
    Task<ApiResponse<IEnumerable<UiInvoiceDto>>> GetUserPaymentInvoicesAsync(string bearerToken, CancellationToken cancellationToken = default);
}
