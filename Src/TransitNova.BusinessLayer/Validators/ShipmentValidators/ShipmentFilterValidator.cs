
using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.Validators.ShipmentValidators
{
    public class ShipmentFilterValidator : AbstractValidator<ShipmentFilterDto>
    {
        private const int MaxPageSize = 30;
        private const int MaxStatusCount =5;
        public ShipmentFilterValidator()
        {
        
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, MaxPageSize)
                .WithMessage($"Page size must be between 1 and {MaxPageSize}.");

          
            RuleFor(x => x.Status)
                .Must(s => s!.Length <= MaxStatusCount)
                .WithMessage($"You can filter by a maximum of {MaxStatusCount} statuses at a time.")
                .Must(s => s!.All(status => Enum.IsDefined(typeof(ShipmentStatuses), status)))
                .WithMessage("One or more provided status values are invalid.")
                .Must(s => s!.Distinct().Count() == s!.Length)
                .WithMessage("Duplicate status values are not allowed.")
                .When(x => x.Status is { Length: > 0 });

   
            RuleFor(x => x.Mode)
                .IsInEnum()
                .WithMessage("Invalid transportation mode value.")
                .When(x => x.Mode.HasValue);

            RuleFor(x => x.From)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("'From' date must not be in the future.")
                .When(x => x.From.HasValue);

            RuleFor(x => x.To)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("'To' date must not be in the future.")
                .When(x => x.To.HasValue);

            RuleFor(x => x)
                .Must(x => x.From <= x.To)
                .WithName("Date Range")
                .WithMessage("'From' date must not be after 'To' date.")
                .When(x => x.From.HasValue && x.To.HasValue);


            RuleFor(x => x.SenderId)
                .NotEqual(Guid.Empty)
                .WithMessage("Sender ID must not be an empty GUID.")
                .When(x => x.SenderId.HasValue);
        }
    }

}
