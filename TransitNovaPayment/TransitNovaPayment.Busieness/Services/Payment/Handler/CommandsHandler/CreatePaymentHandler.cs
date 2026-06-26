using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TransitNovaPayment.Busieness.Common.Abstract.Abstraction.Interfaces.IPaymentService;
using TransitNovaPayment.Busieness.Common.CQRS;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;
using TransitNovaPayment.Busieness.Services.Payment.Command;
namespace TransitNovaPayment.Busieness.Services.Payment.Handler.CommandsHandler
{
    internal sealed class CreatePaymentHandler(IPayment payment,ILogger<CreatePaymentHandler> logger) : ICommandHandler<CreatePaymentCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling command {CommandName} for ShipmentId: {ShipmentId}.",
                nameof(CreatePaymentCommand), request.Dto.ShipmentId);

            var paymentProcessResult = await payment.Pay(request.Dto,request.Key, cancellationToken);

            if (paymentProcessResult is null)
            {
                logger.LogError("Command {CommandName} failed: Payment process returned an unexpected null result for ShipmentId: {ShipmentId}.",
                    nameof(CreatePaymentCommand), request.Dto.ShipmentId);

                return BaseResult.Failure(Errors.Failure("Payment process returned an unexpected null result."));
            }

            if (paymentProcessResult.IsFailure)
            {
                logger.LogWarning("Command {CommandName} completed with failure for ShipmentId: {ShipmentId}.",
                    nameof(CreatePaymentCommand), request.Dto.ShipmentId);

                return paymentProcessResult;
            }

            logger.LogInformation("Command {CommandName} successfully processed for ShipmentId: {ShipmentId}.",
                nameof(CreatePaymentCommand), request.Dto.ShipmentId);

            return paymentProcessResult;
        }
    }
}
