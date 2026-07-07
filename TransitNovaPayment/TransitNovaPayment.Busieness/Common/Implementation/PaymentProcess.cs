using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransitNovaPayment.Busieness.Common.Abstract;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces.IPaymentExecution;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces.IPaymentService;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Repositories.PaymentRepository;
using TransitNovaPayment.Busieness.Common.DTO.PaymentDto;
using TransitNovaPayment.Busieness.Common.Mapping;
using TransitNovaPayment.Busieness.Common.Options;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;
using TransitNovaPayment.Busieness.Contracts.Keys;
using TransitNovaPayment.Busieness.Models.PaymentEntity;
namespace TransitNovaPayment.Busieness.Common.Implementation
{
    internal class PaymentProcess(
        IEnumerable<PaymentMethodService> payments,
        IPaymentCommandRepository paymentRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        IPaymentExecutionStrategy paymentExecutionStrategy,
        IOptions<PaymentGatewaySettings> paymentGatewayOptions,
        ILogger<PaymentProcess> logger) : IPayment
    {
        public async Task<BaseResult?> Pay(CreatePaymentDto dto, string publicKey, CancellationToken cancellationToken)
        {
            logger.LogDebug("Validating payment gateway authentication key.");

            var secretKey = paymentGatewayOptions.Value.PrivateKey;
            if (string.IsNullOrEmpty(secretKey))
            {
                logger.LogCritical("Payment gateway private key is missing from configuration. Key: PaymentSettings:PrivateKey");
                throw new InvalidOperationException("Payment gateway private key is not configured.");
            }

            if (secretKey != publicKey)
            {
                logger.LogWarning("Payment gateway authentication failed due to an invalid API key.");
                return BaseResult.Unauthorized(Errors.UnAuthorized("Invalid payment gateway authentication key."));
            }

            logger.LogDebug("Payment gateway authentication key validated successfully.");
            logger.LogInformation(
                "Starting payment process for ShipmentId: {ShipmentId} using Method: {PaymentMethod}.",
                dto.ShipmentId,
                dto.PaymentMethod);

            var executedService = payments.FirstOrDefault(service => service.PaymentMethod == dto.PaymentMethod);
            if (executedService is null)
            {
                logger.LogError(
                    "Payment failed: Incorrect or unavailable payment method '{PaymentMethod}' for ShipmentId: {ShipmentId}.",
                    dto.PaymentMethod,
                    dto.ShipmentId);

                return BaseResult.Failure(Errors.Failure("Incorrect Payment Method, Payment Method Is Not Available Right Now"));
            }

            var totalAmount = executedService.Pay(dto.ShippingCost, dto.Currency);
            var createPayment = Payment.Create(totalAmount, dto.ShipmentId, dto.PaymentMethod);

            var executionResult = await paymentExecutionStrategy.ExecuteAsync(cancellationToken);
            if (!executionResult.IsSuccess)
            {
                createPayment.MarkAsFailure(executionResult.FailureReason!);
                logger.LogWarning(
                    "Payment TRANSACTION FAILED for ShipmentId: {ShipmentId}. Reason: {FailureReason}.",
                    dto.ShipmentId,
                    executionResult.FailureReason);
            }
            else
            {
                createPayment.MarkAsSucess();
                logger.LogInformation(
                    "Payment TRANSACTION SUCCESSFUL for ShipmentId: {ShipmentId}. Total Amount: {TotalAmount}.",
                    dto.ShipmentId,
                    totalAmount);
            }

            await paymentRepository.CreatePaymentAsync(createPayment, cancellationToken);
            var result = await unitOfWork.SaveChangesAsync(cancellationToken);
            if (result <= 0)
            {
                return BaseResult.Failure(Errors.Failure("Payment Process Failed Due To An Un Excpected Error"));
            }

            await cacheService.RemoveByPrefixAsync(CacheKeys.PaymentsPrefix, cancellationToken);

            var paymentDetails = createPayment.MapToDetailsDto(executedService.Commision);
            logger.LogInformation(
                "Payment process completed smoothly for ShipmentId: {ShipmentId}. PaymentId: {PaymentId}.",
                dto.ShipmentId,
                createPayment.Id);

            return BaseResult.Success(paymentDetails);
        }
    }
}
