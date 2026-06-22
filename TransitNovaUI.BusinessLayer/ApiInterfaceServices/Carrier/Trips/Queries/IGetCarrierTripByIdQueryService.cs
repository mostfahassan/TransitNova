namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Trips.Queries;

public interface IGetCarrierTripByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/carriers/{carrierId:guid}/trips/{tripId:guid}";

    Task<ApiResponse<UiCarrierTripDto>> GetCarrierTripByIdAsync(Guid carrierId, Guid tripId, CancellationToken cancellationToken = default);
}
