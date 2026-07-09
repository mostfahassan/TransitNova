using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace TransitNova.Api.IntegrationTests.Infrastructure;

internal static class JsonContractInspector
{
    internal static async Task<EndpointResponseContractSnapshot> CreateEndpointResponseSnapshotAsync(
        ControllerEndpoint endpoint,
        HttpResponseMessage response)
    {
        var content = response.Content is null
            ? string.Empty
            : await response.Content.ReadAsStringAsync();

        var contentType = response.Content?.Headers.ContentType?.ToString();
        var mediaType = response.Content?.Headers.ContentType?.MediaType;

        return new EndpointResponseContractSnapshot(
            endpoint.HttpMethod,
            endpoint.RouteTemplate,
            endpoint.RequestPath,
            endpoint.EndpointName,
            endpoint.RequiresAuthorization,
            (int)response.StatusCode,
            contentType,
            FlattenJsonPayload(content, mediaType));
    }

    internal static IReadOnlyList<EndpointRequestContractSnapshot> CreateRequestContractSnapshots(IServiceProvider services)
    {
        var endpoints = ControllerEndpointCatalog.Discover(services);
        var serializerOptions = services.GetRequiredService<IOptions<JsonOptions>>().Value.JsonSerializerOptions;

        return endpoints
            .Select(endpoint => new EndpointRequestContractSnapshot(
                endpoint.HttpMethod,
                endpoint.RouteTemplate,
                endpoint.EndpointName,
                endpoint.ActionDescriptor.Parameters
                    .OfType<ControllerParameterDescriptor>()
                    .Where(parameter => parameter.ParameterType != typeof(CancellationToken))
                    .Where(parameter => parameter.ParameterType != typeof(System.Security.Claims.ClaimsPrincipal))
                    .Select(parameter => CreateParameterSnapshot(parameter, endpoint.HttpMethod, serializerOptions))
                    .OrderBy(parameter => parameter.Name, StringComparer.Ordinal)
                    .ToArray()))
            .ToArray();
    }

    internal static IReadOnlyList<string> FlattenJsonPayload(string content, string? mediaType)
    {
        if (string.IsNullOrWhiteSpace(content))
            return ["$: empty"];

        if (!LooksLikeJson(content, mediaType))
            return ["$: text"];

        using var document = JsonDocument.Parse(content);
        var signatures = new List<string>();
        FlattenJsonElement(document.RootElement, "$", signatures);
        return signatures;
    }

    internal static IReadOnlyList<string> FlattenTypeContract(Type type, JsonSerializerOptions serializerOptions)
    {
        var signatures = new List<string>();
        var visited = new HashSet<Type>();
        var nullabilityContext = new NullabilityInfoContext();
        FlattenTypeContract(type, "$", signatures, serializerOptions, visited, nullabilityContext, isNullable: false, isRequired: true);
        return signatures;
    }

    private static ParameterContractSnapshot CreateParameterSnapshot(
        ControllerParameterDescriptor parameter,
        string httpMethod,
        JsonSerializerOptions serializerOptions)
    {
        var bindingSource = parameter.BindingInfo?.BindingSource?.Id ?? InferBindingSource(parameter, httpMethod);
        var isNullable = IsNullableParameter(parameter);
        var signatures = IsSimpleType(parameter.ParameterType)
            ? Array.Empty<string>()
            : FlattenTypeContract(parameter.ParameterType, serializerOptions).ToArray();

        return new ParameterContractSnapshot(
            parameter.Name,
            bindingSource,
            GetFriendlyTypeName(parameter.ParameterType),
            isNullable,
            parameter.ParameterInfo.IsOptional,
            signatures);
    }

    private static void FlattenTypeContract(
        Type type,
        string path,
        List<string> signatures,
        JsonSerializerOptions serializerOptions,
        HashSet<Type> visited,
        NullabilityInfoContext nullabilityContext,
        bool isNullable,
        bool isRequired)
    {
        var unwrappedType = Nullable.GetUnderlyingType(type) ?? type;
        var nullable = isNullable || Nullable.GetUnderlyingType(type) is not null;

        if (TryDescribeScalar(unwrappedType, serializerOptions, out var scalarDescription))
        {
            signatures.Add($"{path}: {scalarDescription} nullable={nullable.ToString().ToLowerInvariant()} required={isRequired.ToString().ToLowerInvariant()}");
            return;
        }

        if (TryGetCollectionElementType(unwrappedType, out var elementType))
        {
            signatures.Add($"{path}: array({GetFriendlyTypeName(unwrappedType)}) nullable={nullable.ToString().ToLowerInvariant()} required={isRequired.ToString().ToLowerInvariant()}");
            FlattenTypeContract(elementType, path + "[*]", signatures, serializerOptions, visited, nullabilityContext, isNullable: false, isRequired: false);
            return;
        }

        signatures.Add($"{path}: object({GetFriendlyTypeName(unwrappedType)}) nullable={nullable.ToString().ToLowerInvariant()} required={isRequired.ToString().ToLowerInvariant()}");

        if (!visited.Add(unwrappedType))
        {
            signatures.Add($"{path}: cycle({GetFriendlyTypeName(unwrappedType)})");
            return;
        }

        foreach (var property in GetSerializableProperties(unwrappedType, serializerOptions))
        {
            var propertyNullability = nullabilityContext.Create(property);
            var propertyNullable = propertyNullability.ReadState == NullabilityState.Nullable || Nullable.GetUnderlyingType(property.PropertyType) is not null;
            var propertyRequired = property.GetCustomAttribute<RequiredAttribute>() is not null ||
                                   property.CustomAttributes.Any(attribute => attribute.AttributeType.FullName == "System.Runtime.CompilerServices.RequiredMemberAttribute");
            var propertyPath = $"{path}.{GetJsonPropertyName(property, serializerOptions)}";

            FlattenTypeContract(
                property.PropertyType,
                propertyPath,
                signatures,
                serializerOptions,
                visited,
                nullabilityContext,
                propertyNullable,
                propertyRequired);
        }

        visited.Remove(unwrappedType);
    }

    private static void FlattenJsonElement(JsonElement element, string path, List<string> signatures)
    {
        switch (element.ValueKind)
        {
            case JsonValueKind.Object:
                signatures.Add($"{path}: object");
                foreach (var property in element.EnumerateObject().OrderBy(property => property.Name, StringComparer.Ordinal))
                    FlattenJsonElement(property.Value, $"{path}.{property.Name}", signatures);
                break;
            case JsonValueKind.Array:
                if (element.GetArrayLength() == 0)
                {
                    signatures.Add($"{path}: array(empty)");
                    break;
                }

                signatures.Add($"{path}: array");
                FlattenJsonElement(element.EnumerateArray().First(), path + "[*]", signatures);
                break;
            case JsonValueKind.String:
                signatures.Add($"{path}: {DescribeStringValue(element.GetString())}");
                break;
            case JsonValueKind.Number:
                signatures.Add($"{path}: {DescribeNumericValue(element)}");
                break;
            case JsonValueKind.True:
            case JsonValueKind.False:
                signatures.Add($"{path}: boolean");
                break;
            case JsonValueKind.Null:
            case JsonValueKind.Undefined:
                signatures.Add($"{path}: null");
                break;
        }
    }

    private static IEnumerable<PropertyInfo> GetSerializableProperties(Type type, JsonSerializerOptions serializerOptions)
    {
        return type
            .GetProperties(BindingFlags.Instance | BindingFlags.Public)
            .Where(property => property.GetMethod is not null && property.GetIndexParameters().Length == 0)
            .Where(property => property.GetCustomAttribute<JsonIgnoreAttribute>() is null)
            .OrderBy(property => GetJsonPropertyName(property, serializerOptions), StringComparer.Ordinal);
    }

    private static string GetJsonPropertyName(PropertyInfo property, JsonSerializerOptions serializerOptions)
    {
        var attributeName = property.GetCustomAttribute<JsonPropertyNameAttribute>()?.Name;
        if (!string.IsNullOrWhiteSpace(attributeName))
            return attributeName;

        return serializerOptions.PropertyNamingPolicy?.ConvertName(property.Name) ?? property.Name;
    }

    private static bool IsNullableParameter(ControllerParameterDescriptor parameter)
    {
        if (Nullable.GetUnderlyingType(parameter.ParameterType) is not null)
            return true;

        if (!parameter.ParameterType.IsValueType)
        {
            var nullabilityContext = new NullabilityInfoContext();
            var nullability = nullabilityContext.Create(parameter.ParameterInfo);
            return nullability.ReadState == NullabilityState.Nullable;
        }

        return false;
    }

    private static string InferBindingSource(ControllerParameterDescriptor parameter, string httpMethod)
    {
        if (parameter.ParameterInfo.GetCustomAttribute<FromRouteAttribute>() is not null)
            return "path";

        if (parameter.ParameterInfo.GetCustomAttribute<FromQueryAttribute>() is not null)
            return "query";

        if (parameter.ParameterInfo.GetCustomAttribute<FromHeaderAttribute>() is not null)
            return "header";

        if (parameter.ParameterInfo.GetCustomAttribute<FromBodyAttribute>() is not null)
            return "body";

        return IsSimpleType(parameter.ParameterType)
            ? "route-or-query"
            : httpMethod is "GET" or "DELETE" ? "query" : "body";
    }

    private static bool IsSimpleType(Type type)
    {
        var unwrappedType = Nullable.GetUnderlyingType(type) ?? type;
        return TryDescribeScalar(unwrappedType, new JsonSerializerOptions(), out _) ||
               unwrappedType == typeof(string);
    }

    private static bool TryDescribeScalar(Type type, JsonSerializerOptions serializerOptions, out string description)
    {
        if (type.IsEnum)
        {
            var serialized = JsonSerializer.Serialize(Enum.GetValues(type).GetValue(0), type, serializerOptions);
            using var document = JsonDocument.Parse(serialized);
            var serializationKind = document.RootElement.ValueKind == JsonValueKind.String ? "string" : "number";
            description = $"enum({type.Name})[{string.Join('|', Enum.GetNames(type))}] serialization={serializationKind}";
            return true;
        }

        description = type switch
        {
            _ when type == typeof(string) => "string",
            _ when type == typeof(Guid) => "guid",
            _ when type == typeof(DateTime) => "date-time",
            _ when type == typeof(DateTimeOffset) => "date-time-offset",
            _ when type == typeof(DateOnly) => "date-only",
            _ when type == typeof(TimeOnly) => "time-only",
            _ when type == typeof(decimal) => "number(decimal)",
            _ when type == typeof(double) || type == typeof(float) => "number",
            _ when type == typeof(byte) ||
                   type == typeof(short) ||
                   type == typeof(int) ||
                   type == typeof(long) ||
                   type == typeof(sbyte) ||
                   type == typeof(ushort) ||
                   type == typeof(uint) ||
                   type == typeof(ulong) => "integer",
            _ when type == typeof(bool) => "boolean",
            _ => string.Empty
        };

        return description.Length > 0;
    }

    private static bool TryGetCollectionElementType(Type type, out Type elementType)
    {
        if (type == typeof(string))
        {
            elementType = typeof(void);
            return false;
        }

        if (type.IsArray)
        {
            elementType = type.GetElementType()!;
            return true;
        }

        var enumerableInterface = type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>)
            ? type
            : type.GetInterfaces().FirstOrDefault(interfaceType =>
                interfaceType.IsGenericType && interfaceType.GetGenericTypeDefinition() == typeof(IEnumerable<>));

        if (enumerableInterface is null)
        {
            elementType = typeof(void);
            return false;
        }

        elementType = enumerableInterface.GetGenericArguments()[0];
        return true;
    }

    private static string DescribeStringValue(string? value)
    {
        if (value is null)
            return "null";

        if (Guid.TryParse(value, out _))
            return "string(guid)";

        if (DateOnly.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            return "string(date-only)";

        if (TimeOnly.TryParseExact(value, "HH:mm:ss.FFFFFFF", CultureInfo.InvariantCulture, DateTimeStyles.None, out _) ||
            TimeOnly.TryParseExact(value, "HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out _))
            return "string(time-only)";

        if (DateTimeOffset.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _))
            return "string(date-time)";

        return "string";
    }

    private static string DescribeNumericValue(JsonElement element)
    {
        if (element.TryGetInt64(out _))
            return "integer";

        return "number(decimal)";
    }

    private static bool LooksLikeJson(string content, string? mediaType)
    {
        if (!string.IsNullOrWhiteSpace(mediaType) &&
            (mediaType.Contains("json", StringComparison.OrdinalIgnoreCase) ||
             mediaType.Contains("problem+json", StringComparison.OrdinalIgnoreCase)))
        {
            return true;
        }

        var trimmed = content.TrimStart();
        return trimmed.StartsWith('{') || trimmed.StartsWith('[');
    }

    private static string GetFriendlyTypeName(Type type)
    {
        if (!type.IsGenericType)
            return type.Name;

        var genericName = type.Name[..type.Name.IndexOf('`')];
        var genericArguments = string.Join(",", type.GetGenericArguments().Select(GetFriendlyTypeName));
        return $"{genericName}<{genericArguments}>";
    }
}

