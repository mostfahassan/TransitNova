namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Commands;

public interface IDeleteCityCommandService
{
    const string HttpMethod = "DELETE";
    const string Route = "api/v{version:apiVersion}/admin/cities/{cityId:int}";

    Task<ApiResponse> DeleteCityAsync(int cityId, CancellationToken cancellationToken = default);
}

