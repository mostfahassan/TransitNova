namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Queries;

public interface IGetVehicleByIdQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/vehicles/{vehicleId:guid}";

    Task<ApiResponse<UiVehicleDto?>> GetVehicleByIdAsync(Guid vehicleId, CancellationToken cancellationToken = default);
}

