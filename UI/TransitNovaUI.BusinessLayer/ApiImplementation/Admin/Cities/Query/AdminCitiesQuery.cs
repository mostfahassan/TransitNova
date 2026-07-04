using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Queries;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Cities.Segregations.Query;
namespace TransitNovaUI.BusinessLayer.ApiImplementation.Admin.Cities.Query
{
    public class AdminCitiesQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IAdminCityQuery
    {
        public Task<ApiResponse<UiPagedResult<UiCityDto>>> FilterCitiesAsync(UiCityFilterDto filter, string bearerToken, CancellationToken cancellationToken = default)
        {
          
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Cities.FilterCitiesUrl,
                ("GovernmentId", filter.GovernmentId), 
                ("SearchTerm", filter.SearchTerm),
                ("PageNumber", filter.PageNumber),
                ("PageSize", filter.PageSize), ("SortDescending", filter.SortDescending)));


            return SendQueryRequestAsync<UiPagedResult<UiCityDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiCityDto?>> GetCityByIdAsync(int cityId, string bearerToken, CancellationToken cancellationToken = default)
        {
            
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Cities.GetCityByIdUrl, ("cityId", cityId)));

            return SendQueryRequestAsync<UiCityDto?>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
