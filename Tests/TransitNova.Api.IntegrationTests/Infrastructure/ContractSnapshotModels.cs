namespace TransitNova.Api.IntegrationTests.Infrastructure;

internal sealed record EndpointResponseContractSnapshot(
    string HttpMethod,
    string RouteTemplate,
    string RequestPath,
    string? EndpointName,
    bool RequiresAuthorization,
    int StatusCode,
    string? ContentType,
    IReadOnlyList<string> Signatures);

internal sealed record EndpointRequestContractSnapshot(
    string HttpMethod,
    string RouteTemplate,
    string? EndpointName,
    IReadOnlyList<ParameterContractSnapshot> Parameters);

internal sealed record ParameterContractSnapshot(
    string Name,
    string Source,
    string ClrType,
    bool IsNullable,
    bool IsOptional,
    IReadOnlyList<string> Signatures);

internal sealed record ErrorScenarioContractSnapshot(
    string Scenario,
    string Endpoint,
    int StatusCode,
    string? ContentType,
    IReadOnlyList<string> Signatures);

internal sealed record SerializerProbeContractSnapshot(
    string Name,
    string Json,
    IReadOnlyList<string> Signatures);
