using System.Security.Cryptography;
using System.Text;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

internal static class ControllerEndpointCatalogSnapshot
{
    internal const int ExpectedEndpointCount = 138;
    internal const string ExpectedSurfaceChecksum = "405EB36565212A28204D33AD0BFED36F3734682CD2D661B96FB7CCE7EBECC672";

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
