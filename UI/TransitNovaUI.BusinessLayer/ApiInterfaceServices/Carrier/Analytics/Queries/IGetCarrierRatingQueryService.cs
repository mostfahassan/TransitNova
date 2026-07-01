namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Analytics.Queries;

public interface IGetCarrierRatingQueryService
{
    Task<ApiResponse<decimal>> GetCarrierRatingAsync(Guid carrierId, string bearerToken, CancellationToken cancellationToken = default);
}

