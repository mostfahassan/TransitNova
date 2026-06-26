using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Http.Json;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Interfaces.PaymentService;
namespace TransitNova.BusinessLayer.Services.PaymentServices
{
    internal class PaymentService(HttpClient client , ILogger<PaymentService> logger,IConfiguration configuration) : IPaymentService
    {
        public async Task<Result<Invoice>> Pay(CreatePaymentDto dto, CancellationToken cancellationToken)
        {
            logger.LogInformation("Payment started for ShipmentId: {ShipmentId}", dto.ShipmentId);

            var key = configuration["PaymentSettings:PublicKey"];
            var baseUrl = configuration["PaymentSettings:BaseUrl"];

            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(baseUrl))
                return Result<Invoice>.Failure(Errors.FailedOperation("Payment configuration missing"));

            using var request = PrepareRequest(dto, key);

            try
            {
                var response = await client.SendAsync(request, cancellationToken);
                var body = await response.Content.ReadAsStringAsync(cancellationToken);

                if (!response.IsSuccessStatusCode)
                    return Result<Invoice>.Failure(Errors.FailedOperation($"Payment failed: {body}"));

                var invoice = await response.Content.ReadFromJsonAsync<Invoice>(cancellationToken);

                if (invoice is null)
                    return Result<Invoice>.Failure(Errors.FailedOperation("Invalid payment response"));

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
            catch (Exception ex)
            {
                logger.LogError(ex, "Payment failure for ShipmentId: {ShipmentId}", dto.ShipmentId);
                return Result<Invoice>.Failure(Errors.FailedOperation("Payment service error"));
            }
        }
        private HttpRequestMessage PrepareRequest(CreatePaymentDto dto,string key)
        {
            var paymentBaseUrl = configuration["PaymentSettings:BaseUrl"];
            if (string.IsNullOrWhiteSpace(paymentBaseUrl))
            {
                throw new InvalidOperationException(
                    "Payment service base URL is not configured.");
            }
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post,$"{paymentBaseUrl}/api/payments/pay" );
            request.Content = JsonContent.Create(dto);
            request.Headers.Add("X-PaymentKey", key);

            return request;
        }
    }
}
