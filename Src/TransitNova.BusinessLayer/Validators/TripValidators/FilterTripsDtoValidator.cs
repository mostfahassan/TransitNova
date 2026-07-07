using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.Domain.Enums.Trip;

namespace TransitNova.BusinessLayer.Validators.TripValidators
{
    public class FilterTripsDtoValidator : AbstractValidator<FilterTripsDto>
    {
        private const int MaxPageSize = 30;
        private const int MaxStatusCount = 5;

        public FilterTripsDtoValidator()
        {
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, MaxPageSize)
                .WithMessage($"Page size must be between 1 and {MaxPageSize}.");

            RuleFor(x => x.Id)
                .NotEqual(Guid.Empty)
                .WithMessage("Trip ID must not be an empty GUID.")
                .When(x => x.Id.HasValue);

            RuleFor(x => x.TripType)
                .IsInEnum()
                .WithMessage("Invalid trip type value.")
                .When(x => x.TripType.HasValue);

            RuleFor(x => x.Status)
                .Must(s => s!.Length <= MaxStatusCount)
                .WithMessage($"You can filter by a maximum of {MaxStatusCount} statuses at a time.")
                .Must(s => s!.All(status => Enum.IsDefined(typeof(TripStatus), status)))
                .WithMessage("One or more provided status values are invalid.")
                .Must(s => s!.Distinct().Count() == s!.Length)
                .WithMessage("Duplicate status values are not allowed.")
                .When(x => x.Status is { Length: > 0 });

            RuleFor(x => x.CreatedAt)
                .LessThanOrEqualTo(DateTime.UtcNow)
                .WithMessage("'CreatedAt' date must not be in the future.")
                .When(x => x.CreatedAt.HasValue);

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

            RuleFor(x => x.CarrierId)
                .NotEqual(Guid.Empty)
                .WithMessage("Carrier ID must not be an empty GUID.")
                .When(x => x.CarrierId.HasValue);

            RuleFor(x => x.WarehouseId)
                .NotEqual(Guid.Empty)
                .WithMessage("Warehouse ID must not be an empty GUID.")
                .When(x => x.WarehouseId.HasValue);

            RuleFor(x => x.HandlerId)
                .NotEqual(Guid.Empty)
                .WithMessage("Handler ID must not be an empty GUID.")
                .When(x => x.HandlerId.HasValue);
        }
    }
}