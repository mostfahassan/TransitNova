using System.Security.Cryptography;
using System.Text;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

internal static class ControllerEndpointCatalogSnapshot
{
    internal const int ExpectedEndpointCount = 116;
    internal const string ExpectedSurfaceChecksum = "020C742CF696D87581EAFC87F8784EC4265B57998604458164191D1D33C03767";

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
