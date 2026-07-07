namespace TransitNovaUI.BusinessLayer.Common.APIHelper.Http
{
    public interface IHttpHandler
    {
      
        HttpRequestMessage RequestBuilder(HttpMethod httpMethod, string url, string? bearerToken, CancellationToken cancellationToken, object? content = null, string? idempotencyKey = null);
        Task<ApiResponse<T>> ReadQueryResponseAsync<T>(HttpResponseMessage httpResponse, CancellationToken ct);
        Task<ApiResponse> ReadCommandResponseAsync(HttpResponseMessage httpResponse, CancellationToken ct);
        Task<ApiResponse<T>> ReadCommandResponseAsync<T>(HttpResponseMessage httpResponse, CancellationToken ct);
        string UrlBuilder(string url);
    }
}
