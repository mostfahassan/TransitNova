using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Features.Payment.Command;
using TransitNova.BusinessLayer.Interfaces.Services.PaymentService;
namespace TransitNova.BusinessLayer.Features.Payment.Handler
{
    public sealed class CreatePaymentHandler(IPaymentService paymentService) : ICommandHandler<CreatePaymentCommand, Result<Invoice>>
    {
        public async Task<Result<Invoice>> Handle(CreatePaymentCommand request, CancellationToken cancellationToken)
        {
            var response = await paymentService.Pay(request.Dto, cancellationToken);

            if (response is null)
                return Result<Invoice>.Failure(Errors.FailedOperation("Payment operation failed"));

            if (response.IsFailure || response.Data is null)
                return Result<Invoice>.Failure(response.Error ?? Errors.FailedOperation(response.Message ?? "Payment operation failed"));

            return Result<Invoice>.Success(response.Data);
        }
    }
}
