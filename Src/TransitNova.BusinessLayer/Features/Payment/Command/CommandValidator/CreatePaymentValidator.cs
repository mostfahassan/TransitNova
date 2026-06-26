using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.Payment.Command.CommandValidator
{
    public class CreatePaymentValidator:AbstractValidator<CreatePaymentCommand>
    {
        public CreatePaymentValidator(IValidator<CreatePaymentDto> dto)
        {
            RuleFor(x => x.Dto)
                .SetValidator(dto);

            RuleFor(x => x.IdempotentKey)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.REQUIRED_FIELD}");
        }
    }
}
