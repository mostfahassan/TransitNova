using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction;
using TransitNovaPayment.Busieness.Common.Contracts.Keys;
using TransitNovaPayment.Busieness.Common.Mapping;
using TransitNovaPayment.Busieness.Common.Options;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;
using TransitNovaPayment.Busieness.DTO.PaymentDto;
using TransitNovaPayment.Busieness.Interfaces.Common;
using TransitNovaPayment.Busieness.Interfaces.PaymentExecution;
using TransitNovaPayment.Busieness.Interfaces.PaymentService;
using TransitNovaPayment.Busieness.Models.PaymentEntity;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
using TransitNovaPayment.Busieness.Repositories.PaymentRepository;

namespace TransitNovaPayment.Busieness.Implementation.PaymentService
{
    public class BundlePaymentProcess(
        IEnumerable<PaymentMethodService> payments,
        IPaymentCommandRepository paymentRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        IPaymentExecutionStrategy paymentExecutionStrategy,
        IOptions<PaymentGatewaySettings> paymentGatewayOptions,
        ILogger<BundlePaymentProcess> logger) : IBundlePayment
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
                "Starting payment process for ReferenceId: {ReferenceId} using Method: {PaymentMethod}.",
                dto.ReferenceId,
                dto.PaymentMethod);

            var executedService = payments.FirstOrDefault(service => service.PaymentMethod == dto.PaymentMethod);
            if (executedService is null)
            {
                logger.LogError(
                    "Payment failed: Incorrect or unavailable payment method '{PaymentMethod}' for ReferenceId: {ReferenceId}.",
                    dto.PaymentMethod,
                    dto.ReferenceId);

                return BaseResult.Failure(Errors.Failure("Incorrect Payment Method, Payment Method Is Not Available Right Now"));
            }

            var totalAmount = executedService.Pay(dto.Cost, dto.Currency);
            var createPayment = Payment.Create(totalAmount, dto.ReferenceId, dto.PaymentMethod, ReferenceType.Bundle);

            var executionResult = await paymentExecutionStrategy.ExecuteAsync(cancellationToken);
            
            
            if (!executionResult.IsSuccess)
            {
                createPayment.MarkAsFailure(executionResult.FailureReason!);
                logger.LogWarning(
                    "Payment TRANSACTION FAILED for ReferenceId: {ReferenceId}. Reason: {FailureReason}.",
                    dto.ReferenceId,
                    executionResult.FailureReason);
            }
            else
            {
                createPayment.MarkAsSucess();
                logger.LogInformation(
                    "Payment TRANSACTION SUCCESSFUL for ReferenceId: {ReferenceId}. Total Amount: {TotalAmount}.",
                    dto.ReferenceId,
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
                "Payment process completed smoothly for ReferenceId: {ReferenceId}. PaymentId: {PaymentId}.",
                dto.ReferenceId,
                createPayment.Id);

            return BaseResult.Success(paymentDetails);
        }
    }
}
