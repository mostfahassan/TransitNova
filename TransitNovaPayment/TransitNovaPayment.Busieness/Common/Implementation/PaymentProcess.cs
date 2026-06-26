
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TransitNovaPayment.Busieness.Common.Abstract;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces.IPaymentService;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Repositories.PaymentRepository;
using TransitNovaPayment.Busieness.Common.DTO.PaymentDto;
using TransitNovaPayment.Busieness.Common.Mapping;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;
using TransitNovaPayment.Busieness.Models.PaymentEntity;
namespace TransitNovaPayment.Busieness.Common.Implementation
{
    internal class PaymentProcess(IEnumerable<PaymentMethodService> Payments,
        IPaymentCommandRepository payment,
        IUnitOfWork unitOfWork,
        IConfiguration configuration,
        ILogger<PaymentProcess> logger) : IPayment
    {
        public async Task<BaseResult?> Pay(CreatePaymentDto dto, string publicKey,CancellationToken cancellationToken)
        {
            logger.LogDebug("Validating payment gateway authentication key.");

            var secretKey = configuration["PaymentSettings:PrivateKey"];
            if (string.IsNullOrEmpty(secretKey))
            {
                logger.LogCritical("Payment gateway private key is missing from configuration. Key: PaymentSettings:PrivateKey");
                throw new ArgumentNullException(nameof(secretKey));
            }
            if (secretKey != publicKey)
            {
                logger.LogWarning("Payment gateway authentication failed due to an invalid API key.");
                return BaseResult.Failure(Errors.UnAuthorized("Invalid payment gateway authentication key."));
            }
            logger.LogDebug("Payment gateway authentication key validated successfully.");


            logger.LogInformation("Starting payment process for ShipmentId: {ShipmentId} using Method: {PaymentMethod}.",
            dto.ShipmentId, dto.PaymentMethod);

            var executedService = Payments
            .FirstOrDefault(s => s.PaymentMethod == dto.PaymentMethod);
            if (executedService is null)
            {
     
                logger.LogError("Payment failed: Incorrect or unavailable payment method '{PaymentMethod}' for ShipmentId: {ShipmentId}.", dto.PaymentMethod, dto.ShipmentId);

                return BaseResult.Failure(Errors.Failure("Incorrect Payment Method, Payment Method Is Not Available Right Now"));
            }

            var totalAmount = executedService.Pay(dto.ShippingCost);
            var createPayment = Payment.Create(totalAmount,dto.ShipmentId, dto.PaymentMethod);
          
            
            await Task.Delay(5000,cancellationToken);
            var isSuccess = new Random().Next(0, 10) > 2;
            if (!isSuccess)
            {
                var failureReason = GetRandomFailureReason();
                createPayment.MarkAsFailure(failureReason); 

                logger.LogWarning("Payment TRANSACTION FAILED for ShipmentId: {ShipmentId}. Reason: {FailureReason}.",
                    dto.ShipmentId, failureReason);
            }
            else
            {
                createPayment.MarkAsSucess();
                logger.LogInformation("Payment TRANSACTION SUCCESSFUL for ShipmentId: {ShipmentId}. Total Amount: {TotalAmount}.",
                    dto.ShipmentId, totalAmount);
            }

            await payment.CreatePaymentAsync(createPayment, cancellationToken);
            var result = await unitOfWork.SaveChangesAsync(cancellationToken);
            if (result > 0) return BaseResult.Failure(Errors.Failure($"Payment Process Failed Due To An Un Excpected Error"));
           

            var paymentDetails = createPayment.MapToDetailsDto(executedService.Commision);

            logger.LogInformation("Payment process completed smoothly for ShipmentId: {ShipmentId}. PaymentId: {PaymentId}.",
                 dto.ShipmentId, createPayment.Id);

            return BaseResult.Success(paymentDetails);
        }
        private static string GetRandomFailureReason()
        {
            string[] reasons = [
            "Insufficient funds in the account.",
            "Card has expired or invalid details provided.",
            "Payment gateway connection timeout.",
            "Transaction declined by the issuing bank."
            ];
            return reasons[new Random().Next(reasons.Length)];
        }
    }
}
