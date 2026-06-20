namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Carriers.Commands;

public interface IDeleteCarrierCommandService
{
    const string HttpMethod = "DELETE";
    const string Route = "api/v{version:apiVersion}/admins/carriers/{id:guid}";

    Task<ApiResponse> DeleteCarrierAsync(Guid id, CancellationToken cancellationToken = default);
}

