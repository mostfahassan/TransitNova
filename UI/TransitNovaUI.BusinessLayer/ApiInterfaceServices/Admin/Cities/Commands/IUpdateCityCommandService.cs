namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Commands;

public interface IUpdateCityCommandService
{
    const string HttpMethod = "PUT";
    const string Route = "api/v{version:apiVersion}/admin/cities/{cityId:int}";

    Task<ApiResponse> UpdateCityAsync(int cityId, UiUpdateCityDto request, CancellationToken cancellationToken = default);
}

