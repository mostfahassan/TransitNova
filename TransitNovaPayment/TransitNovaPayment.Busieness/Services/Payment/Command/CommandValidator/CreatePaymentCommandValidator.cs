using FluentValidation;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult.Enum;
using TransitNovaPayment.Busieness.DTO.PaymentDto;
namespace TransitNovaPayment.Busieness.Services.Payment.Command.CommandValidator
{
    public sealed class CreatePaymentCommandValidator:AbstractValidator<CreateShipmentPaymentCommand>
    {
        public CreatePaymentCommandValidator(IValidator<CreatePaymentDto> dto)
        {
            RuleFor(x => x.Dto)
                .NotEmpty()
                .SetValidator(dto);

            RuleFor(x => x.Key)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");
        }
    }
}
