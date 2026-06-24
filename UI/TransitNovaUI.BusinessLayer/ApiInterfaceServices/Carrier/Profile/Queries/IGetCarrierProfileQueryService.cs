namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Profile.Queries;

public interface IGetCarrierProfileQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/carriers/{carrierId:guid}/profile";

    Task<ApiResponse<UiCarrierProfileDto>> GetCarrierProfileAsync(Guid carrierId, CancellationToken cancellationToken = default);
}

