using FluentValidation;
using TransitNovaPayment.Busieness.Common.DTO.PaymentDto;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult.Enum;

namespace TransitNovaPayment.Busieness.Services.Payment.Command.CommandValidator
{
    public sealed class CreatePaymentCommandValidator:AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentCommandValidator(IValidator<CreatePaymentDto> dto)
        {
            RuleFor(x => x.Dto)
                .NotEmpty()
                .SetValidator(dto);
            
        }
    }
}
