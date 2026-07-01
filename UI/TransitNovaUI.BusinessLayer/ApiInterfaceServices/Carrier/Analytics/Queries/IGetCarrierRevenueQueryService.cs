namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Analytics.Queries;

public interface IGetCarrierRevenueQueryService
{
    Task<ApiResponse<decimal>> GetCarrierRevenueAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default);
}

