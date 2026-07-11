using Microsoft.Extensions.Logging;
using TransitNovaPayment.Busieness.Common.CQRS;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;
using TransitNovaPayment.Busieness.Interfaces.PaymentService;
using TransitNovaPayment.Busieness.Services.Payment.Command;

namespace TransitNovaPayment.Busieness.Services.Payment.Handler.CommandsHandler
{
    public sealed class CreateBundlePaymentCommandHandler(IBundlePayment payment, ILogger<CreateBundlePaymentCommandHandler> logger) : ICommandHandler<CreateBundlePaymentCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(CreateBundlePaymentCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Handling command {CommandName} for BundleId: {BundleId}.",
                nameof(CreateBundlePaymentCommand), request.Dto.ReferenceId);

            var paymentProcessResult = await payment.Pay(request.Dto, request.Key, cancellationToken);

            if (paymentProcessResult is null)
            {
                logger.LogError("Command {CommandName} failed: Payment process returned an unexpected null result for BundleId: {BundleId}.",
                    nameof(CreateBundlePaymentCommand), request.Dto.ReferenceId);

                return BaseResult.Failure(Errors.Failure("Payment process returned an unexpected null result."));
            }

            if (paymentProcessResult.IsFailure)
            {
                logger.LogWarning("Command {CommandName} completed with failure for BundleId: {BundleId}.",
                    nameof(CreateBundlePaymentCommand), request.Dto.ReferenceId);

                return paymentProcessResult;
            }

            logger.LogInformation("Command {CommandName} successfully processed for BundleId: {BundleId}.",
                nameof(CreateBundlePaymentCommand), request.Dto.ReferenceId);

            return paymentProcessResult;
        }
    }
}
