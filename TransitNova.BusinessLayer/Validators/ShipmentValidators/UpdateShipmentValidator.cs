using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Shipment;
namespace TransitNova.BusinessLayer.Validators.ShipmentValidators
{
    public class UpdateShipmentValidator: AbstractValidator<UpdateShipmentDto>
    {
        public UpdateShipmentValidator(IValidator <PackageSpecificationDto> PackageSpecification)
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Shipment Id is required");

            RuleFor(x => x.ReceiverId)
                .NotEmpty().WithMessage("ReceiverId is required");


            RuleFor(x => x.DeliveryAddress)
                .NotEmpty().WithMessage("Delivery address is required")
                .MaximumLength(250);

            RuleFor(x => x.ShipmentType)
                .IsInEnum().WithMessage("Invalid shipment type");
            When(x => x.ShipmentType != null, () =>
            {
                RuleFor(x => x.ShipmentType!)
               .NotEmpty()
               .IsInEnum()
               .WithMessage("Shipment Type Must Be Not Empty");
            });


            When(x => x.TransportationMode.HasValue, () =>
            {
                RuleFor(x => x.TransportationMode!)
                    .IsInEnum()
                    .WithMessage("Invalid transportation mode");
            });


            When(x => !string.IsNullOrEmpty(x.PickupAddress), () =>
            {
                RuleFor(x => x.PickupAddress)
                    .MaximumLength(250).WithMessage("Pickup address must not exceed 250 characters");
            });

            When(x => !string.IsNullOrEmpty(x.DeliveryAddress), () =>
            {
                RuleFor(x => x.DeliveryAddress)
                    .MaximumLength(250).WithMessage("Delivery address must not exceed 250 characters");
            });


            When(x => x.PackageSpecification is not null, () =>
            {
                RuleFor(x => x.PackageSpecification)
                    .SetValidator(PackageSpecification!);
            });
            When(x => x.TransportationMode != null, () =>
            {
                 RuleFor(x => x.TransportationMode!)
                .NotEmpty()
                .IsInEnum()
                .WithMessage("Shipment Type Must Be Not Empty");
            });
        }
    }
    }

