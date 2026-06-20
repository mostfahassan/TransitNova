using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Carrier;
namespace TransitNova.BusinessLayer.Validators.CarrierCompany
{
    public class FilterCarrierDtoValidator : AbstractValidator<FilterCarrierDto>
    {
        public FilterCarrierDtoValidator()
        {          
            RuleFor(x => x.PageNumber)
                .GreaterThanOrEqualTo(1)
                .WithMessage("Page number must be at least 1.");

            RuleFor(x => x.PageSize)
                .InclusiveBetween(1, 100)
                .WithMessage("Page size must be between 1 and 100 to prevent performance issues.");

            // 2. Rating Range Validation
            RuleFor(x => x.MinRating)
                .InclusiveBetween(0m, 5m)
                .When(x => x.MinRating.HasValue)
                .WithMessage("Minimum rating must be between 0 and 5.");

            RuleFor(x => x.MaxRating)
                .InclusiveBetween(0m, 5m)
                .When(x => x.MaxRating.HasValue)
                .WithMessage("Maximum rating must be between 0 and 5.");

            RuleFor(x => x)
                .Must(x => !x.MinRating.HasValue || !x.MaxRating.HasValue || x.MinRating <= x.MaxRating)
                .WithMessage("Minimum rating cannot be greater than maximum rating.");

            //  3. Experience Range Validation
            RuleFor(x => x.MinYearsOfExperience)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinYearsOfExperience.HasValue)
                .WithMessage("Minimum years of experience cannot be negative.");

            RuleFor(x => x.MaxYearsOfExperience)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxYearsOfExperience.HasValue)
                .WithMessage("Maximum years of experience cannot be negative.");

            RuleFor(x => x)
                .Must(x => !x.MinYearsOfExperience.HasValue || !x.MaxYearsOfExperience.HasValue || x.MinYearsOfExperience <= x.MaxYearsOfExperience)
                .WithMessage("Minimum years of experience cannot exceed maximum years.");

            // 🔤 4. Text Fields Validation (حماية من الـ SQL Injection أو الـ Long Strings)
            RuleFor(x => x.City)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.City))
                .WithMessage("City name cannot exceed 100 characters.");

            RuleFor(x => x.SearchTerm)
                .MaximumLength(100)
                .When(x => !string.IsNullOrWhiteSpace(x.SearchTerm))
                .WithMessage("Search term cannot exceed 100 characters.");

            RuleFor(x => x.VehicleCapacityWeight)
                .GreaterThan(0m)
                .When(x => x.VehicleCapacityWeight.HasValue)
                .WithMessage("Vehicle capacity weight must be greater than zero.");

            RuleFor(x => x.VehicleType)
                .IsInEnum()
                .When(x => x.VehicleType.HasValue)
                .WithMessage("Vehicle type is invalid.");

            RuleFor(x => x.ServedZones)
                .Must(zones => zones == null || zones.Count <= 20)
                .WithMessage("Served zones cannot exceed 20 items.");

            RuleForEach(x => x.ServedZones)
                .NotEmpty()
                .WithMessage("Served zone cannot be empty.")
                .MaximumLength(100)
                .WithMessage("Served zone name cannot exceed 100 characters.");

            RuleFor(x => x.ServedZones)
                .Must(zones => zones == null || zones.Distinct(StringComparer.OrdinalIgnoreCase).Count() == zones.Count)
                .WithMessage("Served zones must not contain duplicate values.");

            // 📆 5. Date Validation
            RuleFor(x => x.AvailableFrom)
                .Must(d => d.HasValue && d.Value > DateTime.UtcNow.AddYears(-2))
                .When(x => x.AvailableFrom.HasValue)
                .WithMessage("Available from date cannot be too far in the past.");
        }
    }
}
