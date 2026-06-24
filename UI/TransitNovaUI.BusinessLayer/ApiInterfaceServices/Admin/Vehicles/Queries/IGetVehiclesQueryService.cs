namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Queries;

public interface IGetVehiclesQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/vehicles";

    Task<ApiResponse<List<UiVehicleDto>>> GetVehiclesAsync(CancellationToken cancellationToken = default);
}

