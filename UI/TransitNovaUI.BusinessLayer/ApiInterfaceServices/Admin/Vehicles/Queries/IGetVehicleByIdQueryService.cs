namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Queries;

public interface IGetVehicleByIdQueryService
{
    Task<ApiResponse<UiVehicleDto?>> GetVehicleByIdAsync(Guid vehicleId, string bearerToken, CancellationToken cancellationToken = default);
}

