namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Carrier.Trips.Queries;

public interface IGetCarrierTripsQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/carrier/{carrierId:guid}/trips";

    Task<ApiResponse<IReadOnlyCollection<UiCarrierTripDto>>> GetCarrierTripsAsync(Guid carrierId, CancellationToken cancellationToken = default);
}

