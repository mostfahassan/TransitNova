using System.Net.Http.Json;

namespace TransitNova.E2E.Tests;

internal static class E2EHttp
{
    public static HttpRequestMessage Idempotent(HttpMethod method, string route, object? body = null, Guid? requestId = null)
    {
        var request = new HttpRequestMessage(method, route);
        request.Headers.Add("X-Idempotency-Key", (requestId ?? Guid.NewGuid()).ToString());
        if (body is not null)
            request.Content = JsonContent.Create(body);
        return request;
    }
}
