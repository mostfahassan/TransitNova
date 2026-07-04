namespace TransitNovaUI.BusinessLayer.Common.APIHelper.Http
{
    public abstract class ApiServiceBase(IHttpHandler httpHandler, HttpClient httpClient)
    {
   
        protected async Task<ApiResponse> SendRequestAsync(HttpMethod method, string url, string? bearerToken, CancellationToken cancellationToken, object? content = null, string? idempotencyKey = null)
        {
            var request = httpHandler.RequestBuilder(method, url, bearerToken, cancellationToken, content, idempotencyKey);
            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync(httpResponse, cancellationToken);
        }

        protected async Task<ApiResponse<T>> SendRequestAsync<T>(HttpMethod method, string url, string? bearerToken, CancellationToken cancellationToken, object? content = null, string? idempotencyKey = null)
        {
            var request = httpHandler.RequestBuilder(method, url, bearerToken, cancellationToken, content, idempotencyKey);
            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadCommandResponseAsync<T>(httpResponse, cancellationToken);
        }

        protected async Task<ApiResponse<T>> SendQueryRequestAsync<T>(HttpMethod method, string url, string? bearerToken, CancellationToken cancellationToken, object? content = null, string? idempotencyKey = null)
        {
            var request = httpHandler.RequestBuilder(method, url, bearerToken, cancellationToken, content, idempotencyKey);
            var httpResponse = await httpClient.SendAsync(request, cancellationToken);

            return await httpHandler.ReadQueryResponseAsync<T>(httpResponse, cancellationToken);
        }
    }
}
