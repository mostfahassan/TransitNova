using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Warehouse;

namespace TransitNova.BusinessLayer.Validators.WarehouseValidators
{
    public class UpdateWarehouseDtoValidator : AbstractValidator<UpdateWarehouseDto>
    {
        public UpdateWarehouseDtoValidator()
        {
            RuleFor(w => w.Name)
                .NotEmpty()
                .WithMessage("Warehouse name is required.")
                .MaximumLength(100)
                .WithMessage("Warehouse name must not exceed 100 characters.");

            RuleFor(w => w.Type)
                .IsInEnum()
                .WithMessage("Warehouse type is invalid.");

            RuleFor(w => w.Address)
                .NotEmpty()
                .WithMessage("Warehouse address is required.")
                .MaximumLength(200)
                .WithMessage("Warehouse address must not exceed 200 characters.");

            RuleFor(w => w.Capacity)
                .GreaterThan(0)
                .WithMessage("Warehouse capacity must be greater than zero.");

            RuleFor(w => w.CurrentUsage)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Warehouse current usage cannot be negative.")
                .LessThanOrEqualTo(w => w.Capacity)
                .WithMessage("Warehouse current usage cannot exceed capacity.");

            RuleFor(w => w.OperatingHours)
                .GreaterThan(0)
                .When(w => w.OperatingHours.HasValue)
                .WithMessage("Warehouse operating hours must be greater than zero.");

            RuleFor(w => w.ZoneIds)
                .Cascade(CascadeMode.Stop)
                .NotNull()
                .WithMessage("Warehouse zone ids are required.")
                .Must(zoneIds => zoneIds.Distinct().Count() == zoneIds.Count)
                .WithMessage("Warehouse zone ids must be unique.");
        }
    }
}
