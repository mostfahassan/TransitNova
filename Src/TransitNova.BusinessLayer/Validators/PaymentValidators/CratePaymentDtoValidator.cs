using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Validators.PaymentValidators
{
    public class CratePaymentDtoValidator : AbstractValidator<CreatePaymentDto>
    {

        public CratePaymentDtoValidator()
        {
            RuleFor(x => x.ShipmentId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.REQUIRED_FIELD}");

            RuleFor(x => x.PaymentMethod)
                .IsInEnum()
                .WithErrorCode($"{ErrorCode.REQUIRED_FIELD}");

            RuleFor(x => x.ShippingCost)
                .GreaterThan(0)
                .WithMessage("Shipping cost must be greater than zero.");

            RuleFor(x => x.ShippingCost)
                .LessThanOrEqualTo(100_000)
                .WithMessage("Shipping cost exceeds the allowed limit.");
        }

    }
}
