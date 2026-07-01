namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Queries;

public interface IGetVehicleByPlateNumberQueryService
{
    Task<ApiResponse<UiVehicleDto?>> GetVehicleByPlateNumberAsync(string plateNumber, string bearerToken, CancellationToken cancellationToken = default);
}

