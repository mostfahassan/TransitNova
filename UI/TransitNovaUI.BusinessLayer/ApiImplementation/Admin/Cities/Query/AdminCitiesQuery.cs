using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Segregations.Query;
namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Cities.Query
{
    public class AdminCitiesQuery(IHttpHandler httpHandler, HttpClient httpClient) : IAdminCityQuery
    {
        public async Task<ApiResponse<UiPagedResult<UiCityDto>>> FilterCitiesAsync(UiCityFilterDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Cities.FilterCitiesUrl,
                ("GovernmentId", filter.GovernmentId), 
                ("SearchTerm", filter.SearchTerm),
                ("PageNumber", filter.PageNumber),
                ("PageSize", filter.PageSize), ("SortDescending", filter.SortDescending)));


            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiPagedResult<UiCityDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<UiCityDto?>> GetCityByIdAsync(int cityId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Cities.GetCityByIdUrl, ("cityId", cityId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, url, bearerToken, cancellationToken);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<UiCityDto?>(httpResponse, cancellationToken);
        }

    }
}
