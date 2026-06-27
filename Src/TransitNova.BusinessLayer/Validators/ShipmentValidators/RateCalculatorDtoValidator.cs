using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Shipment;

namespace TransitNova.BusinessLayer.Validators.ShipmentValidators
{
    public sealed class RateCalculatorDtoValidator : AbstractValidator<RateCalculatorDto>
    {
        public RateCalculatorDtoValidator(IValidator<PackageSpecificationDto> packageSpecificationValidator)
        {
            RuleFor(x => x.PackageSpecification)
                .NotNull()
                .WithMessage("Package specification is required.")
                .SetValidator(packageSpecificationValidator);

            RuleFor(x => x.TransportationMode)
                .IsInEnum()
                .WithMessage("Invalid transportation mode.");

            RuleFor(x => x.ShipmentDeliveryType)
                .IsInEnum()
                .WithMessage("Invalid shipment delivery type.");
        }
    }
}
