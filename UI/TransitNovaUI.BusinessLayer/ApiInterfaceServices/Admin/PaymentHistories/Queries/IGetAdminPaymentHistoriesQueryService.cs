using TransitNovaUI.BusinessLayer.DTOs.Payment;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.PaymentHistories.Queries;

public interface IGetAdminPaymentHistoriesQueryService
{
    Task<ApiResponse<UiPagedResult<UiPaymentHistoryDetailsDto>>> GetAdminPaymentHistoriesAsync(
        UiPaymentHistoryFilterDto filter,
        string bearerToken,
        CancellationToken cancellationToken = default);
}
