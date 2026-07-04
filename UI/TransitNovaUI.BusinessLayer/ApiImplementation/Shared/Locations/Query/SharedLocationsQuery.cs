using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Shared.Locations.Query
{
    public class SharedLocationsQuery(IHttpHandler httpHandler, HttpClient httpClient) : ApiServiceBase(httpHandler, httpClient), IGetCitiesByGovernmentQueryService, IGetCountriesQueryService, IGetCountryGovernmentsQueryService, IGetPublicCitiesQueryService, IGetPublicCityByIdQueryService, IGetPublicGovernmentByIdQueryService, IGetPublicGovernmentsQueryService
    {
        public Task<ApiResponse<IEnumerable<UiCityDto>>> GetCitiesByGovernmentAsync(int governmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Locations.GetCitiesByGovernmentUrl, ("governmentId", governmentId)));

            return SendQueryRequestAsync<IEnumerable<UiCityDto>>(HttpMethod.Get, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse<IEnumerable<UiCountryDto>>> GetCountriesAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Locations.GetCountriesUrl));

            return SendQueryRequestAsync<IEnumerable<UiCountryDto>>(HttpMethod.Get, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse<IEnumerable<UiGovernmentDto>>> GetCountryGovernmentsAsync(int countryId, string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Locations.GetCountryGovernmentsUrl, ("countryId", countryId)));

            return SendQueryRequestAsync<IEnumerable<UiGovernmentDto>>(HttpMethod.Get, url, bearerToken, cancellationToken, content, idempotentKey);
        }

        public Task<ApiResponse<UiPagedResult<UiCityDto>>> GetPublicCitiesAsync(UiCityFilterDto filter, string? bearerToken = null, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Locations.GetCitiesUrl, ("GovernmentId", filter.GovernmentId), ("SearchTerm", filter.SearchTerm), ("PageNumber", filter.PageNumber), ("PageSize", filter.PageSize), ("SortDescending", filter.SortDescending)));

            return SendQueryRequestAsync<UiPagedResult<UiCityDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiCityDto?>> GetPublicCityByIdAsync(int cityId, string? bearerToken = null, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Locations.GetCityByIdUrl, ("cityId", cityId)));

            return SendQueryRequestAsync<UiCityDto?>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<UiGovernmentDto?>> GetPublicGovernmentByIdAsync(int governmentId, string? bearerToken = null, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Locations.GetGovernmentByIdUrl, ("governmentId", governmentId)));

            return SendQueryRequestAsync<UiGovernmentDto?>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

        public Task<ApiResponse<List<UiGovernmentDto>>> GetPublicGovernmentsAsync(string? bearerToken = null, CancellationToken cancellationToken = default)
        {
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Locations.GetGovernmentsUrl));

            return SendQueryRequestAsync<List<UiGovernmentDto>>(HttpMethod.Get, url, bearerToken, cancellationToken);
        }

    }
}
