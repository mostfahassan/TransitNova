
using FluentValidation;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.DTOs.Shipment;
namespace TransitNova.BusinessLayer.Validators.ShipmentValidators
{
    public class CreateShipmentValidation : AbstractValidator<CreateShipmentDto>
    {
        public CreateShipmentValidation(IValidator<CreateReceiverDto> dto)
        {
     

            RuleFor(x => x.PackageSpecification)
                .NotNull().WithMessage("Package specification is required.");

            RuleFor(x => x.Currency)
                .IsInEnum().WithMessage("Invalid currency value.");

            RuleFor(x => x.TransportationMode)
                .IsInEnum().WithMessage("Invalid transportation mode.");

            RuleFor(x => x.ShipmentDeliveryType)
                .IsInEnum().WithMessage("Invalid shipment delivery type.");

     
            RuleFor(x => x.DeliveryAddress)
                .NotEmpty().WithMessage("Delivery address is required.")
                .MaximumLength(250).WithMessage("Delivery address cannot exceed 250 characters.");

            RuleFor(x => x.PickupAddress)
                .NotEmpty().WithMessage("Pickup address is required.")
                .MaximumLength(250).WithMessage("Pickup address cannot exceed 250 characters.");

            RuleFor(x => x)
                .Must(x => !x.DeliveryAddress.Equals(x.PickupAddress, StringComparison.OrdinalIgnoreCase))
                .WithMessage("Delivery and pickup addresses cannot be identical.")
                .When(x => !string.IsNullOrWhiteSpace(x.DeliveryAddress) && !string.IsNullOrWhiteSpace(x.PickupAddress));

            RuleFor(x => x.PackageBundleId)
                .NotEmpty()
                .WithMessage("Package bundle Cant Be Empty.");

            RuleFor(x => x.Receiver)
                .NotNull()
                .SetValidator(dto);
        }
    }

}
