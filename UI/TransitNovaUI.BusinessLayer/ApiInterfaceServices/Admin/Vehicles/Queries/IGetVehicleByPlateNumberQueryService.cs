namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Queries;

public interface IGetVehicleByPlateNumberQueryService
{
    const string HttpMethod = "GET";
    const string Route = "api/v{version:apiVersion}/admin/vehicles/plate-number/{plateNumber}";

    Task<ApiResponse<UiVehicleDto?>> GetVehicleByPlateNumberAsync(string plateNumber, CancellationToken cancellationToken = default);
}

