using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using System.Text.Json.Serialization;
using TransitNova.Domain.Enums.Result;

namespace TransitNovaUI.BusinessLayer.Common.APIHelper.Http
{
    public class HttpHandler(ILogger<HttpHandler> logger) : IHttpHandler
    {
        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

   
        public async Task<ApiResponse<T>> ReadQueryResponseAsync<T>(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            var response = await TryReadResponseAsync<T>(httpResponse, ct);
            return response ?? ApiResponse<T>.FailedResponse(Errors.Failure("Failed to read Query response."));
        }

        public async Task<ApiResponse> ReadCommandResponseAsync(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            var response = await TryReadResponseAsync(httpResponse, ct);
            return response ?? ApiResponse.Failure(Errors.Failure("Failed to read command response."));
        }

        public async Task<ApiResponse<T>> ReadCommandResponseAsync<T>(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            var response = await TryReadResponseAsync<T>(httpResponse, ct);
            return response ?? ApiResponse<T>.FailedResponse(Errors.Failure("Failed to read command response."));
        }

        public HttpRequestMessage RequestBuilder(HttpMethod httpMethod, string url, string? bearerToken, CancellationToken cancellationToken, object? content = null, string? idempotencyKey = null)
        {
            var request = new HttpRequestMessage(httpMethod, url);

            if (content is not null && httpMethod != HttpMethod.Get && httpMethod != HttpMethod.Head)
            {
                request.Content = JsonContent.Create(content, options: _jsonOptions);
            }

            if (!string.IsNullOrWhiteSpace(idempotencyKey))
            {
                request.Headers.Add(ApiHelper.IdempotentHeader, idempotencyKey);
            }

            if (!string.IsNullOrWhiteSpace(bearerToken))
            {
                request.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", bearerToken);
            }
            return request;
        }

        public string UrlBuilder(string url) => $"{ApiHelper.BaseUrl}/{url.TrimStart('/')}";


        private async Task<ApiResponse<T>?> TryReadResponseAsync<T>(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            var body = await httpResponse.Content.ReadAsStringAsync(ct);

            if (string.IsNullOrWhiteSpace(body))
                return ToResponse<T>(ResolveEmptyBodyOutcome(httpResponse));

            if (TryParseProblemDetails(httpResponse, body, out var problemOutcome))
                return ToResponse<T>(problemOutcome);

            try
            {
                return JsonSerializer.Deserialize<ApiResponse<T>>(body, _jsonOptions);
            }
            catch (Exception ex) when (ex is JsonException or NotSupportedException or InvalidOperationException)
            {
                LogDeserializationFailure(ex, httpResponse);
                return ToResponse<T>(BuildInvalidJsonOutcome(httpResponse, body));
            }
        }

        private async Task<ApiResponse?> TryReadResponseAsync(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            var body = await httpResponse.Content.ReadAsStringAsync(ct);

            if (string.IsNullOrWhiteSpace(body))
                return ToResponse(ResolveEmptyBodyOutcome(httpResponse));

            if (TryParseProblemDetails(httpResponse, body, out var problemOutcome))
                return ToResponse(problemOutcome);

            try
            {
                return JsonSerializer.Deserialize<ApiResponse>(body, _jsonOptions);
            }
            catch (Exception ex) when (ex is JsonException or NotSupportedException or InvalidOperationException)
            {
                LogDeserializationFailure(ex, httpResponse);
                return ToResponse(BuildInvalidJsonOutcome(httpResponse, body));
            }
        }

        // ---- Outcome -> response mapping (compile-time typed, zero reflection) ----

        // Plain data holder describing "what the response should look like".
        // It carries no knowledge of ApiResponse vs ApiResponse<T>, so the parsing
        // logic below (empty body / ProblemDetails / invalid JSON) is written ONCE
        // and reused by both the generic and non-generic read paths.
        private readonly record struct ResponseOutcome(
            bool IsSuccess,
            ResultStatus Status,
            int StatusCode,
            ErrorCode? ErrorCode,
            ApiError? Error,
            IReadOnlyList<ApiError> Errors,
            string? Message);

        private static ApiResponse<T> ToResponse<T>(ResponseOutcome outcome) =>
            new(default, outcome.IsSuccess, outcome.Status, outcome.StatusCode, outcome.ErrorCode, outcome.Error, outcome.Errors, outcome.Message);

        private static ApiResponse ToResponse(ResponseOutcome outcome) =>
            new(outcome.IsSuccess, outcome.Status, outcome.StatusCode, outcome.ErrorCode, outcome.Error, outcome.Errors, outcome.Message);

        // ---- Outcome builders (pure functions, no reflection - each mirrors the original logic 1:1) ----

        private static ResponseOutcome ResolveEmptyBodyOutcome(HttpResponseMessage httpResponse)
        {
            var statusCode = (int)httpResponse.StatusCode;
            var status = ResolveStatus(statusCode);

            if (httpResponse.IsSuccessStatusCode)
                return new ResponseOutcome(true, status, statusCode, null, null, Array.Empty<ApiError>(), null);

            var message = $"Request failed with HTTP {statusCode}.";
            var error = Errors.Failure(message);
            return new ResponseOutcome(false, status, statusCode, error.Code, error, [error], message);
        }

        private static ResponseOutcome BuildInvalidJsonOutcome(HttpResponseMessage httpResponse, string body)
        {
            var statusCode = (int)httpResponse.StatusCode;

            var preview = body.Trim();
            if (preview.Length > 180)
                preview = preview[..180];

            var message = $"Unexpected API response format. HTTP {statusCode}. Body starts with: {preview}";
            var error = Errors.Failure(message);

            return new ResponseOutcome(false, ResolveStatus(statusCode), statusCode, error.Code, error, [error], message);
        }

        private static bool TryParseProblemDetails(HttpResponseMessage httpResponse, string body, out ResponseOutcome outcome)
        {
            outcome = default;
            try
            {
                using var document = JsonDocument.Parse(body);
                var root = document.RootElement;

                if (root.ValueKind != JsonValueKind.Object || !LooksLikeProblemDetails(root))
                    return false;

                var statusCode = ReadInt(root, "statusCode") ?? ReadInt(root, "status") ?? (int)httpResponse.StatusCode;
                var message = ReadString(root, "message")
                    ?? ReadString(root, "title")
                    ?? ReadString(root, "detail")
                    ?? $"Request failed with HTTP {statusCode}.";

                var errorCode = ReadErrorCode(root, "errorCode") ?? ErrorCode.FAILED;
                var errors = ReadErrors(root, errorCode);
                var error = errors.FirstOrDefault() ?? new ApiError(errorCode, message);

                outcome = new ResponseOutcome(false, ResolveStatus(statusCode), statusCode, error.Code, error, errors, message);
                return true;
            }
            catch (JsonException)
            {
                return false;
            }
        }

        private static bool LooksLikeProblemDetails(JsonElement root) =>
            !root.TryGetProperty("isSuccess", out _)
            && (root.TryGetProperty("title", out _)
                || root.TryGetProperty("detail", out _)
                || root.TryGetProperty("type", out _)
                || root.TryGetProperty("errors", out _));

        private static IReadOnlyList<ApiError> ReadErrors(JsonElement root, ErrorCode fallbackCode)
        {
            if (!root.TryGetProperty("errors", out var errorsElement))
                return [];

            var errors = new List<ApiError>();

            if (errorsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var errorElement in errorsElement.EnumerateArray())
                {
                    var code = ReadErrorCode(errorElement, "code") ?? fallbackCode;
                    var message = ReadString(errorElement, "message");
                    if (!string.IsNullOrWhiteSpace(message))
                        errors.Add(new ApiError(code, message));
                }
            }
            else if (errorsElement.ValueKind == JsonValueKind.Object)
            {
                foreach (var property in errorsElement.EnumerateObject())
                {
                    if (property.Value.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var messageElement in property.Value.EnumerateArray())
                        {
                            var message = messageElement.ValueKind == JsonValueKind.String
                                ? messageElement.GetString()
                                : messageElement.ToString();

                            if (!string.IsNullOrWhiteSpace(message))
                                errors.Add(new ApiError(ErrorCode.VALIDATION_ERROR, $"{property.Name}: {message}"));
                        }
                    }
                    else
                    {
                        var message = property.Value.ValueKind == JsonValueKind.String
                            ? property.Value.GetString()
                            : property.Value.ToString();

                        if (!string.IsNullOrWhiteSpace(message))
                            errors.Add(new ApiError(ErrorCode.VALIDATION_ERROR, $"{property.Name}: {message}"));
                    }
                }
            }

            return errors;
        }

        private static string? ReadString(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var value) || value.ValueKind == JsonValueKind.Null)
                return null;

            return value.ValueKind == JsonValueKind.String ? value.GetString() : value.ToString();
        }

        private static int? ReadInt(JsonElement root, string propertyName)
        {
            if (!root.TryGetProperty(propertyName, out var value))
                return null;

            if (value.ValueKind == JsonValueKind.Number && value.TryGetInt32(out var number))
                return number;

            return value.ValueKind == JsonValueKind.String && int.TryParse(value.GetString(), out number) ? number : null;
        }

        private static ErrorCode? ReadErrorCode(JsonElement root, string propertyName)
        {
            var value = ReadString(root, propertyName);
            return Enum.TryParse<ErrorCode>(value, ignoreCase: true, out var errorCode) ? errorCode : null;
        }

        private void LogDeserializationFailure(Exception ex, HttpResponseMessage httpResponse)
        {
            logger.LogWarning(ex,
                "Failed to deserialize API response. StatusCode: {StatusCode}. Body starts with: {BodyPreview}",
                (int)httpResponse.StatusCode,
                httpResponse.Content.Headers.ContentType?.MediaType);
        }

        private static ResultStatus ResolveStatus(int statusCode) => statusCode switch
        {
            200 => ResultStatus.Success,
            201 => ResultStatus.Created,
            204 => ResultStatus.NoContent,
            401 => ResultStatus.Unauthorized,
            403 => ResultStatus.Forbidden,
            404 => ResultStatus.NotFound,
            409 => ResultStatus.Conflict,
            422 => ResultStatus.ValidationError,
            500 => ResultStatus.UnExpected,
            _ => statusCode >= 500 ? ResultStatus.UnExpected : ResultStatus.Failure
        };
    }
}