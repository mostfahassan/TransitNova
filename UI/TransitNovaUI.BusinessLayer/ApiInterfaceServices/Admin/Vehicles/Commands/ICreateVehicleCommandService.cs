namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Commands;

public interface ICreateVehicleCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/v{version:apiVersion}/admin/vehicles";

    Task<ApiResponse<UiVehicleDto>> CreateVehicleAsync(UiCreateVehicleDto request, CancellationToken cancellationToken = default);
}
