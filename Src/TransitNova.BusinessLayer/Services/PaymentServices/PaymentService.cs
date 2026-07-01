using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using System.Text.Json;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Options;
using TransitNova.BusinessLayer.Interfaces.Services.PaymentService;
namespace TransitNova.BusinessLayer.Services.PaymentServices
{
    internal class PaymentService(HttpClient client, ILogger<PaymentService> logger, IOptions<PaymentSettings> paymentOptions) : IPaymentService
    {
        private static readonly JsonSerializerOptions PaymentJsonOptions = new(JsonSerializerDefaults.Web);

        public async Task<Result<Invoice>> Pay(CreatePaymentDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("Payment started for ShipmentId: {ShipmentId}", dto.ShipmentId);

            var settings = paymentOptions.Value;
            var key = settings.PublicKey;
            var baseUrl = settings.BaseUrl;

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(baseUrl))
                return Result<Invoice>.Failure(Errors.FailedOperation("Payment configuration missing"));

            using var request = PrepareRequest(dto, settings);

            try
            {
                var response = await client.SendAsync(request, cancellationToken);
                
                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                var gatewayResult = JsonSerializer.Deserialize<PaymentGatewayResponse>(body, PaymentJsonOptions);

                if (gatewayResult is null)
                    return Result<Invoice>.Failure(Errors.FailedOperation("Invalid payment response"));

                if (!response.IsSuccessStatusCode || gatewayResult.IsFailure || !gatewayResult.IsSuccess)
                    return Result<Invoice>.Failure(Errors.FailedOperation(GetPaymentFailureMessage(gatewayResult)));

                if (gatewayResult.Data is null)
                    return Result<Invoice>.Failure(Errors.FailedOperation("Invalid payment response"));

                var invoice = gatewayResult.Data.ToInvoice(dto.ShippingCost);

                if (string.Equals(invoice.Status, "Failed", StringComparison.OrdinalIgnoreCase))
                    return Result<Invoice>.Failure(Errors.FailedOperation(invoice.Notes ?? "Payment transaction failed"));

                return Result<Invoice>.Success(invoice);
            }
            catch (TaskCanceledException)
            {
                logger.LogError("Payment request timeout for ShipmentId: {ShipmentId}", dto.ShipmentId);
                return Result<Invoice>.Failure(Errors.FailedOperation("Payment timeout"));
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "HTTP error during payment for ShipmentId: {ShipmentId}", dto.ShipmentId);
                return Result<Invoice>.Failure(Errors.FailedOperation("Payment service unreachable"));
            }
            catch (JsonException ex)
            {
                logger.LogError(ex, "Invalid payment response for ShipmentId: {ShipmentId}", dto.ShipmentId);
                return Result<Invoice>.Failure(Errors.FailedOperation("Invalid payment response"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Payment failure for ShipmentId: {ShipmentId}", dto.ShipmentId);
                return Result<Invoice>.Failure(Errors.FailedOperation("Payment service error"));
            }
        }


        private static HttpRequestMessage PrepareRequest(CreatePaymentDto dto, PaymentSettings settings)
        {
            if (string.IsNullOrWhiteSpace(settings.BaseUrl))
            {
                throw new InvalidOperationException(
                    "Payment service base URL is not configured.");
            }
            var baseUrl = settings.BaseUrl.TrimEnd('/');
            HttpRequestMessage request = new(HttpMethod.Post, $"{baseUrl}/api/v1/payments/pay");
            request.Content = JsonContent.Create(dto);
            request.Headers.Add("X-PaymentKey", settings.PublicKey);
            return request;
        }

        private static string GetPaymentFailureMessage(PaymentGatewayResponse gatewayResult)
        {
            return gatewayResult.Error?.Message
                ?? gatewayResult.Message
                ?? "Payment failed";
        }

        private sealed class PaymentGatewayResponse
        {
            public PaymentGatewayInvoice? Data { get; init; }
            public bool IsSuccess { get; init; }
            public bool IsFailure { get; init; }
            public string? Message { get; init; }
            public int StatusCode { get; init; }
            public PaymentGatewayError? Error { get; init; }
        }

        private sealed class PaymentGatewayInvoice
        {
            public Guid PaymentId { get; init; }
            public Guid ShipmentId { get; init; }
            public decimal Amount { get; init; }
            public decimal Commission { get; init; }
            public decimal TotalAmount { get; init; }
            public string PaymentMethod { get; init; } = string.Empty;
            public string Status { get; init; } = string.Empty;
            public DateTime? PaidAt { get; init; }
            public string? Notes { get; init; }

            public Invoice ToInvoice(decimal shippingCost)
            {
                return new Invoice
                {
                    PaymentId = PaymentId,
                    ShipmentId = ShipmentId,
                    ShippingCost = shippingCost,
                    Commission = Commission,
                    Amount = TotalAmount,
                    PaymentMethod = PaymentMethod,
                    Status = Status,
                    PaidAt = PaidAt,
                    Notes = Notes
                };
            }
        }

        private sealed class PaymentGatewayError
        {
            public string? Message { get; init; }
        }
    }
}
