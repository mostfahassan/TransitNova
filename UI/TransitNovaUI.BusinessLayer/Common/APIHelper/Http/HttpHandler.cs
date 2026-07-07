using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using TransitNova.Domain.Enums.Result;

namespace TransitNovaUI.BusinessLayer.Common.APIHelper.Http
{
    public class HttpHandler : IHttpHandler
    {
        private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public async Task<ApiResponse<T>> ReadQueryResponseAsync<T>(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            var response = await TryReadResponseAsync<ApiResponse<T>>(httpResponse, ct);
            return response ?? ApiResponse<T>.FailedResponse(Errors.Failure("Failed to read Query response."));
        }

        public async Task<ApiResponse> ReadCommandResponseAsync(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            var response = await TryReadResponseAsync<ApiResponse>(httpResponse, ct);
            return response ?? ApiResponse.Failure(Errors.Failure("Failed to read command response."));
        }

        public async Task<ApiResponse<T>> ReadCommandResponseAsync<T>(HttpResponseMessage httpResponse, CancellationToken ct)
        {
            var response = await TryReadResponseAsync<ApiResponse<T>>(httpResponse, ct);
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

        private static async Task<TResponse?> TryReadResponseAsync<TResponse>(HttpResponseMessage httpResponse, CancellationToken ct)
            where TResponse : ApiResponse
        {
            var body = await httpResponse.Content.ReadAsStringAsync(ct);
            if (string.IsNullOrWhiteSpace(body))
                return null;

            var problemDetailsResponse = TryReadProblemDetailsResponse<TResponse>(httpResponse, body);
            if (problemDetailsResponse is not null)
                return problemDetailsResponse;

            try
            {
                return JsonSerializer.Deserialize<TResponse>(body, _jsonOptions);
            }
            catch (JsonException)
            {
                return CreateInvalidJsonResponse<TResponse>(httpResponse, body);
            }
        }
        private static TResponse? TryReadProblemDetailsResponse<TResponse>(HttpResponseMessage httpResponse, string body)
            where TResponse : ApiResponse
        {
            try
            {
                using var document = JsonDocument.Parse(body);
                var root = document.RootElement;

                if (root.ValueKind != JsonValueKind.Object || !LooksLikeProblemDetails(root))
                    return null;

                var statusCode = ReadInt(root, "statusCode") ?? ReadInt(root, "status") ?? (int)httpResponse.StatusCode;
                var message = ReadString(root, "message")
                    ?? ReadString(root, "title")
                    ?? ReadString(root, "detail")
                    ?? $"Request failed with HTTP {statusCode}.";

                var errorCode = ReadErrorCode(root, "errorCode") ?? ErrorCode.FAILED;
                var errors = ReadErrors(root, errorCode);
                var error = errors.FirstOrDefault() ?? new ApiError(errorCode, message);

                return CreateFailureResponse<TResponse>(statusCode, message, error, errors);
            }
            catch (JsonException)
            {
                return null;
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

        private static TResponse CreateInvalidJsonResponse<TResponse>(HttpResponseMessage httpResponse, string body)
            where TResponse : ApiResponse
        {
            var preview = body.Trim();
            if (preview.Length > 180)
                preview = preview[..180];

            var message = $"Unexpected API response format. HTTP {(int)httpResponse.StatusCode}. Body starts with: {preview}";
            var error = Errors.Failure(message);

            return CreateFailureResponse<TResponse>((int)httpResponse.StatusCode, message, error, [error]);
        }

        private static TResponse CreateFailureResponse<TResponse>(int statusCode, string message, ApiError error, IReadOnlyList<ApiError> errors)
            where TResponse : ApiResponse
        {
            var status = ResolveStatus(statusCode);

            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(ApiResponse<>))
            {
                var dataType = typeof(TResponse).GetGenericArguments()[0];
                var responseType = typeof(ApiResponse<>).MakeGenericType(dataType);
                return (TResponse)Activator.CreateInstance(responseType, null, false, status, statusCode, error.Code, error, errors, message)!;
            }

            return (TResponse)(ApiResponse)new(false, status, statusCode, error.Code, error, errors, message);
        }

        private static ResultStatus ResolveStatus(int statusCode)
        {
            return statusCode switch
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
}