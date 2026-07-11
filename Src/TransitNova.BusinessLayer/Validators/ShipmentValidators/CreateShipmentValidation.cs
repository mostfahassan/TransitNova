
using FluentValidation;
using TransitNova.BusinessLayer.Common.CommonData;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Validators.AddressValidator;
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
                .NotNull().WithMessage("Delivery address is required.")
                .SetValidator(new AddressDtoValidator());

            RuleFor(x => x.PickupAddress)
                .NotNull().WithMessage("Pickup address is required.")
                .SetValidator(new AddressDtoValidator());

            RuleFor(x => x)
                .Must(x => !string.Equals(x.DeliveryAddress.ToNormalizedString(), x.PickupAddress.ToNormalizedString(), StringComparison.Ordinal))
                .WithMessage("Delivery and pickup addresses cannot be identical.")
                .When(x => x.DeliveryAddress is not null && x.PickupAddress is not null);

            

            RuleFor(x => x.Receiver)
                .NotNull()
                .SetValidator(dto);
        }
    }

}
