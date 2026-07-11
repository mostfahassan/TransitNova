using FluentValidation;
using TransitNova.BusinessLayer.Common.CommonData;
namespace TransitNova.BusinessLayer.Validators.BaseValidators
{
    public class BaseBundleValidator<T> : AbstractValidator<T> where T : CommonBundleData
    {

        public BaseBundleValidator()
        {
            
            RuleFor(x => x.BundleName)
                .NotEmpty().WithMessage("Bundle name is required.")
                .Length(3, 100).WithMessage("Bundle name must be between 3 and 100 characters.");

            RuleFor(x => x.BundleDescription)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.");

            RuleFor(x => x.BundlePrice)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.BundleDurationMonths)
                .InclusiveBetween(1, 24).WithMessage("Duration must be between 1 and 24 months.");

          
            RuleFor(x => x.MaxShipmentsPerMonth)
                .GreaterThanOrEqualTo(0).WithMessage("Max shipments cannot be negative.");

            RuleFor(x => x.MaxWeightPerShipment)
                .GreaterThan(0).WithMessage("Max weight per shipment must be greater than zero.");

            RuleFor(x => x.MaxDistancePerShipment)
                .GreaterThan(0).WithMessage("Max distance per shipment must be greater than zero.");

            RuleFor(x => x.DiscountPercentage)
                .InclusiveBetween(0, 100).WithMessage("Discount percentage must be between 0 and 100.");

            RuleFor(x => x.MinimumShipmentValueForDiscount)
                .GreaterThanOrEqualTo(0).WithMessage("Minimum shipment value cannot be negative.");
        }
    }
}
