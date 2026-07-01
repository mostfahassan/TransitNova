using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Reflection.Metadata;
using System.Text.Json;

namespace TransitNovaUI.BusinessLayer.Common.APIHelper.Http
{
    public class HttpHandler : IHttpHandler
    {
        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() }
        };

        public async Task<ApiResponse<T>> ReadQueryResponseAsync<T>(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = await httpResponse.Content.ReadAsStringAsync(ct);

                return new ApiResponse<T>
                {
                    Success = false,
                    StatusCode = (int)httpResponse.StatusCode,
                    Message = $"API error ({(int)httpResponse.StatusCode}): {error}"
                };
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<T>>(_jsonOptions, ct);
            if (response is null)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    StatusCode = (int)httpResponse.StatusCode,
                    Message = "Empty response from API"
                };
            }
            return response;
        }

        public async Task<ApiResponse> ReadCommandResponseAsync(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = await httpResponse.Content.ReadAsStringAsync(ct);

                return new ApiResponse
                {
                    Success = false,
                    StatusCode = (int)httpResponse.StatusCode,
                    Message = $"API error ({(int)httpResponse.StatusCode}): {error}"
                };
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse>(_jsonOptions, ct);

            if (response is null)
            {
                return new ApiResponse
                {
                    Success = false,
                    StatusCode = (int)httpResponse.StatusCode,
                    Message = "Empty response from API"          
                };
            }
            return response;
        }

        public async Task<ApiResponse<T>> ReadCommandResponseAsync<T>(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            if (!httpResponse.IsSuccessStatusCode)
            {
                var error = await httpResponse.Content.ReadAsStringAsync(ct);

                return new ApiResponse<T>
                {
                    Success = false,
                    StatusCode = (int)httpResponse.StatusCode,
                    Message = $"API error ({(int)httpResponse.StatusCode}): {error}"
                };
            }

            var response = await httpResponse.Content.ReadFromJsonAsync<ApiResponse<T>>(_jsonOptions, ct);

            if (response is null)
            {
                return new ApiResponse<T>
                {
                    Success = false,
                    StatusCode = (int)httpResponse.StatusCode,
                    Message = "Empty response from API"
                };
            }
            return response;
        }

        public HttpRequestMessage RequestBuilder(HttpMethod httpMethod, string url, string? bearerToken, CancellationToken cancellationToken,object? content = null, string? idempotencyKey = null)
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
