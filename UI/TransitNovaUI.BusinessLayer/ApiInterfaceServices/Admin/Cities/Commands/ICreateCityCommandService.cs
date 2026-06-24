namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Commands;

public interface ICreateCityCommandService
{
    const string HttpMethod = "POST";
    const string Route = "api/v{version:apiVersion}/admin/cities";

    Task<ApiResponse<UiCityDto>> CreateCityAsync(UiCreateCityDto request, CancellationToken cancellationToken = default);
}

