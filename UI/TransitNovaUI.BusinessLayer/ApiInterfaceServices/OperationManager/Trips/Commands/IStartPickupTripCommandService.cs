namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Commands;

public interface IStartPickupTripCommandService
{
    const string HttpMethod = "PATCH";
    const string Route = "api/v{version:apiVersion}/operation-managers/trips/{carrierId:guid}/start-pickup";

    Task<ApiResponse> StartPickupTripAsync(Guid carrierId, CancellationToken cancellationToken = default);
}
