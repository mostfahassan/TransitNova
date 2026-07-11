using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Validators.AddressValidator;

namespace TransitNova.BusinessLayer.Validators.ShipmentValidators
{
    public class UpdateShipmentValidator : AbstractValidator<UpdateShipmentDto>
    {
        public UpdateShipmentValidator(IValidator<PackageSpecificationDto> packageSpecification)
        {
            RuleFor(x => x.ReceiverId)
                .NotEmpty().WithMessage("ReceiverId is required");

            When(x => x.DeliveryAddress is not null, () =>
            {
                RuleFor(x => x.DeliveryAddress!)
                    .SetValidator(new AddressDtoValidator());
            });

            When(x => x.PickupAddress is not null, () =>
            {
                RuleFor(x => x.PickupAddress!)
                    .SetValidator(new AddressDtoValidator());
            });

            When(x => x.ShipmentType.HasValue, () =>
            {
                RuleFor(x => x.ShipmentType!)
                    .IsInEnum()
                    .WithMessage("Invalid shipment type");
            });

            When(x => x.TransportationMode.HasValue, () =>
            {
                RuleFor(x => x.TransportationMode!)
                    .IsInEnum()
                    .WithMessage("Invalid transportation mode");
            });

            When(x => x.PackageSpecification is not null, () =>
            {
                RuleFor(x => x.PackageSpecification!)
                    .SetValidator(packageSpecification);
            });
        }
    }
}