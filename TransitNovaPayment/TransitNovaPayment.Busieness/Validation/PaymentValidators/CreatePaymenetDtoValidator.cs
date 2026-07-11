using FluentValidation;
using TransitNovaPayment.Busieness.DTO.PaymentDto;

namespace TransitNovaPayment.Busieness.Validation.PaymentValidators
{
    public class CreatePaymenetDtoValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymenetDtoValidator()
        {
            RuleFor(x => x.ReferenceId)
                .NotEmpty()
                .WithMessage("ReferenceId is required and cannot be an empty GUID.");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum()
                .WithMessage("The selected payment method is invalid or not supported.");

            RuleFor(x => x.Cost)
                .GreaterThan(0)
                .WithMessage("Shipping cost must be greater than zero.")
                .PrecisionScale(18, 2, true)
                .WithMessage("Shipping cost cannot exceed 2 decimal places with a maximum precision of 18 digits.");
        }
    }
}
