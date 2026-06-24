namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Queries;

public interface IGetActiveVehiclesQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/vehicles/active";

    Task<ApiResponse<List<UiVehicleDto>>> GetActiveVehiclesAsync(CancellationToken cancellationToken = default);
}

