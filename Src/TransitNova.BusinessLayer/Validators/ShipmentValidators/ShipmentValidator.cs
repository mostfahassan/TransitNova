using FluentValidation;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums;

public class ShipmentValidator : AbstractValidator<Shipment>
{
    public ShipmentValidator()
    {
        RuleFor(x => x.DeliveryAddress)
            .NotNull()
            .ChildRules(ValidateAddress);

        RuleFor(x => x.PickupAddress)
            .NotNull()
            .ChildRules(ValidateAddress);

        RuleFor(x => x.TrackingNumber)
            .NotEmpty().WithMessage("TrackingNumber is required.")
            .MaximumLength(100).WithMessage("TrackingNumber must not exceed 100 characters.");

        RuleFor(x => x.ReceiverId)
            .NotEmpty().WithMessage("ReceiverId is required.");

        RuleFor(x => x.SenderId)
            .NotEmpty().WithMessage("SenderId is required.");

        RuleFor(x => x.Receiver)
            .NotNull().When(x => x.ReceiverId != Guid.Empty)
            .WithMessage("Receiver must exist.");

        RuleFor(x => x.Sender)
            .NotNull().When(x => x.SenderId != Guid.Empty)
            .WithMessage("Sender must exist.");

        RuleFor(x => x.ShipmentCost)
            .GreaterThan(0).WithMessage("ShipmentCost must be greater than zero.");

        RuleFor(x => x.Currency)
            .IsInEnum().WithMessage("Currency is invalid.");

        RuleFor(x => x.PackageSpecification)
            .NotNull().WithMessage("PackageSpecification is required.");


        RuleFor(x => x.PickupDate).Null().When(x => x.IsCancelled || x.IsDeleted)
            .WithMessage("PickupDate must be null for cancelled or deleted shipments.")
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date).When(x => !x.IsCancelled && !x.IsDeleted)
            .WithMessage("PickupDate must be today or in the future for active shipments.");


        RuleFor(x => x.EstimatedDeliveryDate)
            .NotNull().When(x => x.IsCancelled == false)
            .WithMessage("EstimatedDeliveryDate is required for active shipments.");

        RuleFor(x => x.ActualDeliveryDate)
            .GreaterThanOrEqualTo(x => x.PickupDate!.Value)
            .When(x => x.ActualDeliveryDate.HasValue && x.PickupDate.HasValue)
            .WithMessage("ActualDeliveryDate must be after or equal to PickupDate.");

        RuleFor(x => x.EstimatedDeliveryDate)
            .GreaterThanOrEqualTo(x => x.PickupDate!.Value)
            .When(x => x.EstimatedDeliveryDate.HasValue && x.PickupDate.HasValue)
            .WithMessage("EstimatedDeliveryDate must be after or equal to PickupDate.");

        RuleFor(x => x.CancelledOn)
            .NotNull()
            .When(x => x.IsCancelled)
            .WithMessage("CancelledOn is required when shipment is cancelled.");

        RuleFor(x => x.DeletedOn)
            .NotNull()
            .When(x => x.IsDeleted)
            .WithMessage("DeletedOn is required when shipment is deleted.");

        RuleFor(x => x.HandlerId)
            .NotEmpty()
            .When(x => x.HandlerId.HasValue)
            .WithMessage("HandlerId is invalid.");

        RuleFor(x => x.CurrentStatus)
            .IsInEnum().WithMessage("CurrentStatus is invalid.");
    }

    private static void ValidateAddress(InlineValidator<Address> validator)
    {
        validator.RuleFor(address => address.MainAddress)
            .NotEmpty()
            .MaximumLength(Address.MainAddressMaxLength);

        validator.RuleFor(address => address.SecondaryAddress)
            .MaximumLength(Address.SecondaryAddressMaxLength)
            .When(address => !string.IsNullOrWhiteSpace(address.SecondaryAddress));

        validator.RuleFor(address => address.Street)
            .NotEmpty()
            .MaximumLength(Address.StreetMaxLength);
    }
}