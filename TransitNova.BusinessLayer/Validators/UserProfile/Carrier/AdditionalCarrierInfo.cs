using FluentValidation;
using System;
using TransitNova.BusinessLayer.DTOs.Carrier;

namespace TransitNova.BusinessLayer.Validators.UserProfile.Carrier
{
    public class AdditionalCarrierInfo: AbstractValidator<AdditionalInfoDto>
    {
        public AdditionalCarrierInfo()
        {
            RuleFor(x => x.LicenseNumber)
          .NotEmpty()
          .WithMessage("License number is required.")
          .MaximumLength(50)
          .WithMessage("License number cannot exceed 50 characters.");

            RuleFor(x => x.MaxDailyShipments)
                .GreaterThan(0)
                .WithMessage("Maximum daily shipments must be greater than zero.");

       
            RuleFor(x => x.DefaultCostPerKg)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Default cost per kilogram must be zero or a positive value.");

            RuleFor(x => x.YearsOfExperience)
                .GreaterThanOrEqualTo(0)
                .WithMessage("Years of experience must be zero or a positive value.");
            RuleFor(x => x.ContractYears)
                .GreaterThan(0)
                .WithMessage("Contract Years of experience must be Greater Than Zero");

            RuleFor(x => x.ContractStartDate)
                .NotEmpty()
                .GreaterThan(DateTime.UtcNow)
                .LessThan(DateTime.UtcNow.AddMonths(1))
                .WithMessage("Start date must be in the future.");
        }
    }
}
