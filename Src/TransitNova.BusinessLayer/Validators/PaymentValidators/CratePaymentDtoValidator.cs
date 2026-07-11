using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Validators.PaymentValidators
{
    public class CratePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
    {

        public CratePaymentDtoValidator()
        {
            RuleFor(x => x.ReferenceId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.REQUIRED_FIELD}");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum()
                .WithErrorCode($"{ErrorCode.REQUIRED_FIELD}");

            RuleFor(x => x.Cost)
                .GreaterThan(0)
                .WithMessage("Cost must be greater than zero.");

            RuleFor(x => x.Currency)
                .IsInEnum()
                .WithMessage("Currency must be a valid enum value.");

        }

    }
}
