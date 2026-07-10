using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Interfaces.Services.PaymentService;
using TransitNova.BusinessLayer.Options;
namespace TransitNova.BusinessLayer.Services.PaymentServices
{
    internal sealed class PaymentHistoryService(HttpClient client,ILogger<PaymentHistoryService> logger,IOptions<PaymentSettings> paymentOptions)
        : IPaymentHistoryService
    {
        private static readonly JsonSerializerOptions PaymentJsonOptions = new(JsonSerializerDefaults.Web)
        {
            Converters = { new JsonStringEnumConverter() }
        };

        public async Task<Result<PagedResult<PaymentHistoryDetailsDto>>> GetPaymentHistoriesAsync(PaymentHistoryFilterDto filter, CancellationToken cancellationToken)
        {
            logger.LogInformation(
                "Fetching payment histories. PageNumber: {PageNumber}, PageSize: {PageSize}",
                filter.PageNumber,
                filter.PageSize);

            var settings = paymentOptions.Value;
            if (string.IsNullOrWhiteSpace(settings.PublicKey) || string.IsNullOrWhiteSpace(settings.BaseUrl))
                return Result<PagedResult<PaymentHistoryDetailsDto>>.Failure(
                    Errors.FailedOperation("Payment configuration missing"));

            using var request = PrepareRequest(filter, settings);

            try
            {
                using var response = await client.SendAsync(request, cancellationToken);
                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    logger.LogWarning(
                        "Payment history request failed with status code {StatusCode}.",
                        (int)response.StatusCode);

                    return Result<PagedResult<PaymentHistoryDetailsDto>>.Failure(
                        Errors.FailedOperation(GetFailureMessage(body, (int)response.StatusCode)));
                }

                var remoteResult = JsonSerializer.Deserialize<RemotePagedResult<PaymentHistoryDetailsDto>>(
                    body,
                    PaymentJsonOptions);

                if (remoteResult is null)
                    return Result<PagedResult<PaymentHistoryDetailsDto>>.Failure(
                        Errors.FailedOperation("Invalid payment history response"));

                var pagedResult = PagedResult<PaymentHistoryDetailsDto>.From(
                    remoteResult.Data ?? [],
                    remoteResult.TotalCount,
                    remoteResult.PageNumber,
                    remoteResult.PageSize);

                return Result<PagedResult<PaymentHistoryDetailsDto>>.Success(pagedResult);
            }
            catch (TaskCanceledException)
            {
                logger.LogError(
                    "Payment history request timeout. PageNumber: {PageNumber}, PageSize: {PageSize}",
                    filter.PageNumber,
                    filter.PageSize);

                return Result<PagedResult<PaymentHistoryDetailsDto>>.Failure(
                    Errors.FailedOperation("Payment history timeout"));
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "HTTP error while fetching payment histories.");
                return Result<PagedResult<PaymentHistoryDetailsDto>>.Failure(
                    Errors.FailedOperation("Payment service unreachable"));
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Invalid payment history response.");
                return Result<PagedResult<PaymentHistoryDetailsDto>>.Failure(
                    Errors.FailedOperation("Invalid payment history response"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Payment history retrieval failed.");
                return Result<PagedResult<PaymentHistoryDetailsDto>>.Failure(
                    Errors.FailedOperation("Payment history service error"));
            }
        }

        private static HttpRequestMessage PrepareRequest(PaymentHistoryFilterDto filter, PaymentSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.BaseUrl))
                throw new InvalidOperationException("Payment service base URL is not configured.");

            var queryString = BuildQueryString(filter);
            var baseUrl = settings.BaseUrl.TrimEnd('/');
            var request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}/api/v1/payments/history{queryString}");
            request.Headers.Add("X-PaymentKey", settings.PublicKey);
            return request;
        }

        private static string BuildQueryString(PaymentHistoryFilterDto filter)
        {
            var query = new List<string>();

            AddQueryValue(query, "PaymentId", filter.PaymentId);
            AddQueryValue(query, "PaymentStatus", filter.PaymentStatus);
            AddQueryValue(query, "PaymentMethod", filter.PaymentMethod);
            AddQueryValue(query, "CreatedAt", filter.CreatedAt);
            AddQueryValue(query, "CreatedAtFrom", filter.CreatedAtFrom);
            AddQueryValue(query, "CreatedAtTo", filter.CreatedAtTo);
            AddQueryValue(query, "CreatedBy", filter.CreatedBy);
            AddQueryValue(query, "PageNumber", filter.PageNumber);
            AddQueryValue(query, "PageSize", filter.PageSize);

            return query.Count == 0 ? string.Empty : $"?{string.Join("&", query)}";
        }

        private static void AddQueryValue(List<string> query, string name, object? value)
        {
            if (value is null)
                return;

            var formattedValue = value switch
            {
                DateTime dateTime => dateTime.ToString("O", CultureInfo.InvariantCulture),
                DateTimeOffset dateTimeOffset => dateTimeOffset.ToString("O", CultureInfo.InvariantCulture),
                bool boolean => boolean.ToString().ToLowerInvariant(),
                IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture),
                _ => value.ToString()
            };

            if (string.IsNullOrWhiteSpace(formattedValue))
                return;

            query.Add($"{Uri.EscapeDataString(name)}={Uri.EscapeDataString(formattedValue)}");
        }

        private static string GetFailureMessage(string body, int statusCode)
        {
            if (string.IsNullOrWhiteSpace(body))
                return $"Payment history request failed with HTTP {statusCode}.";

            try
            {
                using var document = JsonDocument.Parse(body);
                var root = document.RootElement;

                if (TryReadString(root, "message", out var message) && !string.IsNullOrWhiteSpace(message))
                    return message;

                if (TryReadString(root, "detail", out var detail) && !string.IsNullOrWhiteSpace(detail))
                    return detail;

                if (TryReadString(root, "title", out var title) && !string.IsNullOrWhiteSpace(title))
                    return title;
            }
            catch (JsonException)
            {
            }

            return $"Payment history request failed with HTTP {statusCode}.";
        }

        private static bool TryReadString(JsonElement element, string propertyName, out string? value)
        {
            value = null;

            if (!element.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
                return false;

            value = property.ValueKind == JsonValueKind.String ? property.GetString() : property.ToString();
            return true;
        }

        private sealed class RemotePagedResult<T>
        {
            public IEnumerable<T>? Data { get; set; }
            public int TotalCount { get; set; }
            public int PageNumber { get; set; }
            public int PageSize { get; set; }
        }
    }
}
