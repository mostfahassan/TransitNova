namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Vehicles.Commands;

public interface IDeleteVehicleCommandService
{
    const string HttpMethod = "DELETE";
    const string Route = "api/v{version:apiVersion}/admin/vehicles/{vehicleId:guid}";

    Task<ApiResponse> DeleteVehicleAsync(Guid vehicleId, CancellationToken cancellationToken = default);
}

