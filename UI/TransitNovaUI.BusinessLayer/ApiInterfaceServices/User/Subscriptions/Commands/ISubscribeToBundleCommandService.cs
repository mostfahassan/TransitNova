using TransitNovaUI.BusinessLayer.DTOs.Payment;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.User.Subscriptions.Commands;

public interface ISubscribeToBundleCommandService
{
    Task<ApiResponse<UiBundleInvoiceDto>> SubscribeToBundleAsync(Guid bundleId, UiSubscribeToBundleDto dto, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

