using System.Security.Cryptography;
using System.Text;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

internal static class ControllerEndpointCatalogSnapshot
{
    internal const int ExpectedEndpointCount = 140;
    internal const string ExpectedSurfaceChecksum = "EDAE93F258FD5BC70226C1A458A9AA6CD90342C08760F670244634B0195E9846";

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


