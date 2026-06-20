
using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Carrier;

namespace TransitNova.BusinessLayer.Validators.UserProfile.Carrier
{
    public class FilterCarrierDtoValidator : AbstractValidator<FilterCarrierDto>
    {
        private const int MaxPageSize = 100;
        private const decimal MinAllowedRating = 0m;
        private const decimal MaxAllowedRating = 5m;
        private const int MinAllowedYears = 0;
        private const int MaxAllowedYears = 100;
        private const decimal MinVehicleCapacity = 0.1m;
        private const decimal MaxVehicleCapacity = 100_000m;
        private const int MaxSearchTermLength = 100;
        private const int MaxCityLength = 100;
        private const int MaxZonesCount = 20;
        private const int MaxZoneNameLength = 100;

        public FilterCarrierDtoValidator()
        {
           
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, MaxPageSize)
                .WithMessage($"Page size must be between 1 and {MaxPageSize}.");

           
            RuleFor(x => x.MinRating)
                .InclusiveBetween(MinAllowedRating, MaxAllowedRating)
                .WithMessage($"Minimum rating must be between {MinAllowedRating} and {MaxAllowedRating}.")
                .When(x => x.MinRating.HasValue);

            RuleFor(x => x.MaxRating)
                .InclusiveBetween(MinAllowedRating, MaxAllowedRating)
                .WithMessage($"Maximum rating must be between {MinAllowedRating} and {MaxAllowedRating}.")
                .When(x => x.MaxRating.HasValue);

            RuleFor(x => x)
                .Must(x => x.MinRating <= x.MaxRating)
                .WithName("Rating Range")
                .WithMessage("Minimum rating must not exceed maximum rating.")
                .When(x => x.MinRating.HasValue && x.MaxRating.HasValue);

            RuleFor(x => x.MinYearsOfExperience)
                .InclusiveBetween(MinAllowedYears, MaxAllowedYears)
                .WithMessage($"Minimum years of experience must be between {MinAllowedYears} and {MaxAllowedYears}.")
                .When(x => x.MinYearsOfExperience.HasValue);

            RuleFor(x => x.MaxYearsOfExperience)
                .InclusiveBetween(MinAllowedYears, MaxAllowedYears)
                .WithMessage($"Maximum years of experience must be between {MinAllowedYears} and {MaxAllowedYears}.")
                .When(x => x.MaxYearsOfExperience.HasValue);

            RuleFor(x => x)
                .Must(x => x.MinYearsOfExperience <= x.MaxYearsOfExperience)
                .WithName("Experience Range")
                .WithMessage("Minimum years of experience must not exceed maximum years of experience.")
                .When(x => x.MinYearsOfExperience.HasValue && x.MaxYearsOfExperience.HasValue);

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid carrier status value.")
                .When(x => x.Status.HasValue);

            
            RuleFor(x => x.CompanyId)
                .NotEqual(Guid.Empty)
                .WithMessage("Company ID must not be an empty GUID.")
                .When(x => x.CompanyId.HasValue);

            
            RuleFor(x => x.City)
                .NotEmpty()
                .WithMessage("City name must not be empty.")
                .MaximumLength(MaxCityLength)
                .WithMessage($"City name must not exceed {MaxCityLength} characters.")
                .Matches(@"^[\p{L}\s\-'\.]+$")
                .WithMessage("City name contains invalid characters.")
                .When(x => x.City is not null);

            RuleFor(x => x.CityId)
                .GreaterThan(0)
                .WithMessage("City ID must be a positive integer.")
                .When(x => x.CityId.HasValue);

            RuleFor(x => x)
                .Must(x => !(x.City is not null && x.CityId.HasValue))
                .WithName("City Filter")
                .WithMessage("Provide either City name or City ID, not both.");

         
            RuleFor(x => x.SearchTerm)
                .NotEmpty()
                .WithMessage("Search term must not be empty or whitespace.")
                .MaximumLength(MaxSearchTermLength)
                .WithMessage($"Search term must not exceed {MaxSearchTermLength} characters.")
                .When(x => x.SearchTerm is not null);

            
            RuleFor(x => x.AvailableFrom)
                .GreaterThanOrEqualTo(DateTime.UtcNow.Date)
                .WithMessage("Available from date must not be in the past.")
                .When(x => x.AvailableFrom.HasValue);

           
            RuleFor(x => x.VehicleCapacityWeight)
                .InclusiveBetween(MinVehicleCapacity, MaxVehicleCapacity)
                .WithMessage($"Vehicle capacity must be between {MinVehicleCapacity} kg and {MaxVehicleCapacity} kg.")
                .When(x => x.VehicleCapacityWeight.HasValue);

            RuleFor(x => x.VehicleType)
                .IsInEnum()
                .WithMessage("Invalid vehicle type value.")
                .When(x => x.VehicleType.HasValue);

          
            RuleFor(x => x.ServedZones)
                .Must(z => z!.Count <= MaxZonesCount)
                .WithMessage($"You can filter by a maximum of {MaxZonesCount} zones at a time.")
                .Must(z => z!.All(zone => !string.IsNullOrWhiteSpace(zone)))
                .WithMessage("Zone names must not be empty or whitespace.")
                .Must(z => z!.All(zone => zone.Length <= MaxZoneNameLength))
                .WithMessage($"Each zone name must not exceed {MaxZoneNameLength} characters.")
                .Must(z => z!.Distinct(StringComparer.OrdinalIgnoreCase).Count() == z!.Count)
                .WithMessage("Duplicate zone names are not allowed.")
                .When(x => x.ServedZones is { Count: > 0 });

        
            RuleFor(x => x.SortBy)
                .IsInEnum()
                .WithMessage("Invalid sort field value.")
                .When(x => x.SortBy.HasValue);

            RuleFor(x => x.SortDescending)
                .Equal(false)
                .WithMessage("SortDescending has no effect when SortBy is not specified.")
                .When(x => !x.SortBy.HasValue && x.SortDescending);
        }
    }
}
