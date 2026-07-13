using System.Security.Cryptography;
using System.Text;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

internal static class ControllerEndpointCatalogSnapshot
{
    internal const int ExpectedEndpointCount = 142;
    internal const string ExpectedSurfaceChecksum = "69B9A3878F79A85974754B4E0383D969BCDCE3AEE6A017A3AF2BE4C2EE86BD12";

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






