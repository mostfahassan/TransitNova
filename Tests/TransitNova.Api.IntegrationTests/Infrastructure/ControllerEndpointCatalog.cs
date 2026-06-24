using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Patterns;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

internal sealed record ControllerEndpoint(
    string HttpMethod,
    string RouteTemplate,
    string RequestPath,
    string? EndpointName,
    bool RequiresAuthorization,
    ControllerActionDescriptor ActionDescriptor);

internal static class ControllerEndpointCatalog
{
    internal static IReadOnlyCollection<ControllerEndpoint> Discover(IServiceProvider services)
    {
        var dataSources = services.GetServices<EndpointDataSource>();

        return dataSources
            .SelectMany(source => source.Endpoints)
            .OfType<RouteEndpoint>()
            .Select(endpoint => new
            {
                Endpoint = endpoint,
                Action = endpoint.Metadata.GetMetadata<ControllerActionDescriptor>(),
                Methods = endpoint.Metadata.GetMetadata<IHttpMethodMetadata>()?.HttpMethods
            })
            .Where(item => item.Action is not null && item.Methods is not null)
            .SelectMany(item => item.Methods!.Select(method => new ControllerEndpoint(
                method,
                item.Endpoint.RoutePattern.RawText ?? string.Empty,
                BuildRequestPath(item.Endpoint.RoutePattern),
                item.Endpoint.Metadata.GetMetadata<IEndpointNameMetadata>()?.EndpointName,
                item.Endpoint.Metadata.GetOrderedMetadata<IAuthorizeData>().Count > 0 &&
                item.Endpoint.Metadata.GetMetadata<IAllowAnonymous>() is null,
                item.Action!)))
            .OrderBy(endpoint => endpoint.RouteTemplate, StringComparer.Ordinal)
            .ThenBy(endpoint => endpoint.HttpMethod, StringComparer.Ordinal)
            .ToArray();
    }

    private static string BuildRequestPath(RoutePattern pattern)
    {
        var segments = pattern.PathSegments.Select(segment =>
            string.Concat(segment.Parts.Select(BuildPart)));

        return "/" + string.Join('/', segments);
    }

    private static string BuildPart(RoutePatternPart part) => part switch
    {
        RoutePatternLiteralPart literal => literal.Content,
        RoutePatternSeparatorPart separator => separator.Content,
        RoutePatternParameterPart parameter => BuildParameter(parameter),
        _ => string.Empty
    };

    private static string BuildParameter(RoutePatternParameterPart parameter)
    {
        if (parameter.Name.Equals("version", StringComparison.OrdinalIgnoreCase))
            return "1";

        if (parameter.ParameterPolicies.Any(policy =>
                policy.Content?.Contains("int", StringComparison.OrdinalIgnoreCase) == true))
            return "1";

        if (parameter.ParameterPolicies.Any(policy =>
                policy.Content?.Contains("guid", StringComparison.OrdinalIgnoreCase) == true) ||
            parameter.Name.EndsWith("Id", StringComparison.OrdinalIgnoreCase))
            return "11111111-1111-1111-1111-111111111111";

        if (parameter.Name.Contains("tracking", StringComparison.OrdinalIgnoreCase))
            return "TN-2026-000001";

        if (parameter.Name.Contains("plate", StringComparison.OrdinalIgnoreCase))
            return "ABC-123";

        return WebUtility.UrlEncode("sample");
    }
}
