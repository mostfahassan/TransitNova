using TransitNovaUI.BusinessLayer.Common.APIHelper.Http;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Locations.Queries;

namespace TransitNovaUI.BusinessLayer.ApiImplementation.Shared.Locations.Query
{
    public class SharedLocationsQuery(IHttpHandler httpHandler, HttpClient httpClient) : IGetCitiesByGovernmentQueryService, IGetCountriesQueryService, IGetCountryGovernmentsQueryService
    {
        public async Task<ApiResponse<IEnumerable<UiCityDto>>> GetCitiesByGovernmentAsync(int governmentId, string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Locations.GetCitiesByGovernmentUrl, ("governmentId", governmentId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, content, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<IEnumerable<UiCityDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<UiCountryDto>>> GetCountriesAsync(string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Locations.GetCountriesUrl));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, content, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<IEnumerable<UiCountryDto>>(httpResponse, cancellationToken);
        }

        public async Task<ApiResponse<IEnumerable<UiGovernmentDto>>> GetCountryGovernmentsAsync(int countryId, string bearerToken, CancellationToken cancellationToken = default)
        {
            object? content = null;
            string? idempotentKey = null;
            var url = httpHandler.UrlBuilder(ApiRoutes.Build(ApiRoutes.Locations.GetCountryGovernmentsUrl, ("countryId", countryId)));

            var request = httpHandler.RequestBuilder(HttpMethod.Get, content, url, bearerToken, cancellationToken, idempotentKey);

            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<IEnumerable<UiGovernmentDto>>(httpResponse, cancellationToken);
        }

    }
}
