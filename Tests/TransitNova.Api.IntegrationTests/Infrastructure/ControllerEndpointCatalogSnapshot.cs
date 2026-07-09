using System.Security.Cryptography;
using System.Text;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

internal static class ControllerEndpointCatalogSnapshot
{
    internal const int ExpectedEndpointCount = 135;
    internal const string ExpectedSurfaceChecksum = "0D3143E845BE66A244EC80B25C3253CDE588DFFA4DC4ED52D5DC12FE73CE7841";

    internal static string[] CreateSurfaceSignatures(IEnumerable<ControllerEndpoint> endpoints)
    {
        return endpoints
            .Select(endpoint => string.Join(
                " | ",
                endpoint.HttpMethod,
                endpoint.RouteTemplate,
                endpoint.RequiresAuthorization ? "auth" : "public",
                endpoint.EndpointName ?? string.Empty))
            .OrderBy(signature => signature, StringComparer.Ordinal)
            .ToArray();
    }

    internal static string ComputeChecksum(IEnumerable<string> signatures)
    {
        var payload = string.Join('\n', signatures);
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash);
    }
}




