namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Commands;

public interface ICreateVehicleCommandService
{
    Task<ApiResponse<UiVehicleDto>> CreateVehicleAsync(UiCreateVehicleDto model, string bearerToken, string idempotentKey, CancellationToken cancellationToken = default);
}

