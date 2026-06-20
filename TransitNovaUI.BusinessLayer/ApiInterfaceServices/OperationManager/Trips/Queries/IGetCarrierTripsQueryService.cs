namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Queries;

public interface IGetCarrierTripsQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/operation-manager/carriers/{carrierId:guid}/trips";

    Task<ApiResponse<IReadOnlyCollection<UiCarrierTripDto>>> GetCarrierTripsAsync(Guid carrierId, CancellationToken cancellationToken = default);
}

