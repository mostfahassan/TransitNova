using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace TransitNovaUI.BusinessLayer.Common.APIHelper.Http
{
    public class HttpHandler : IHttpHandler
    {
        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };
        public async Task<ApiResponse<T>> ReadQueryResponseAsync<T>(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<T>>(_jsonOptions, ct);
            return response ?? ApiResponse<T>.FailedResponse(Errors.Failure("Failed to read Query response."));
        }
        public async Task<ApiResponse> ReadCommandResponseAsync(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions, ct);
            return response ?? ApiResponse.Failure(Errors.Failure("Failed to read command response."));

        }
        public async Task<ApiResponse<T>> ReadCommandResponseAsync<T>(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<T>>(_jsonOptions, ct);

            return response ?? ApiResponse<T>.FailedResponse(Errors.Failure("Failed to read command response."));
        }

        public HttpRequestMessage RequestBuilder(HttpMethod httpMethod, object? content, string url, string? bearerToken, CancellationToken cancellationToken, string? idempotencyKey = null) =>
            RequestBuilder(httpMethod, url, bearerToken, cancellationToken, content, idempotencyKey);

        public HttpRequestMessage RequestBuilder(HttpMethod httpMethod, string url, string? bearerToken, CancellationToken cancellationToken, object? content = null, string? idempotencyKey = null)
        {
            var request = new HttpRequestMessage(httpMethod, url);

            if (content is not null && httpMethod != HttpMethod.Get && httpMethod != HttpMethod.Head)
            {
                request.Content = JsonContent.Create(content, options: _jsonOptions);
            }

            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                request.Headers.Add(ApiHelper.IdempotentHeader, idempotencyKey);
            }

            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearerToken);
            }
            return request;
        }

        public string UrlBuilder(string url) => $"{ApiHelper.BaseUrl}/{url.TrimStart('/')}";
    }
}
