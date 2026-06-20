namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.OperationManager.Trips.Commands;

public interface IStartDeliveryTripCommandService
{
    const string HttpMethod = "PATCH";
    const string Route = "api/v{version:apiVersion}/operation-manager/trips/{carrierId:guid}/start-delivery";

    Task<ApiResponse> StartDeliveryTripAsync(Guid carrierId, CancellationToken cancellationToken = default);
}

