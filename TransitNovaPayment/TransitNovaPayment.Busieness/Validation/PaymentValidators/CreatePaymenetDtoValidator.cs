
using FluentValidation;
using TransitNovaPayment.Busieness.Common.DTO.PaymentDto;
namespace TransitNovaPayment.Busieness.Validation.PaymentValidators
{
    public class CreatePaymenetDtoValidator : AbstractValidator<CreatePaymentDto>
    {
        public CreatePaymenetDtoValidator()
        {
            RuleFor(x => x.ShipmentId)
               .NotEmpty()
               .WithMessage("ShipmentId is required and cannot be an empty GUID.");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum()
                .WithMessage("The selected payment method is invalid or not supported.");

            RuleFor(x => x.ShippingCost)
                .GreaterThan(0)
                .WithMessage("Shipping cost must be greater than zero.")
                .PrecisionScale(2, 18, true)
                .WithMessage("Shipping cost cannot exceed 2 decimal places with a maximum precision of 18 digits.");
        }
    }
}
