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
                .MaximumLength(100).WithMessage("Bundle name must not exceed 100 characters.");
            RuleFor(x => x.TotalWeight)
                .GreaterThan(0).WithMessage("Total weight must be greater than zero.");
            RuleFor(x => x.BundlePrice)
                .GreaterThan(0).WithMessage("Bundle price must be greater than zero.");
            RuleFor(x => x.BundleDescription)
                .MaximumLength(500).WithMessage("Bundle description must not exceed 500 characters.");
            RuleFor(x => x.TotalDistance)
                .GreaterThan(0).WithMessage("Total distance must be greater than zero.");
            RuleFor(x => x.TotalShipments)
                .GreaterThan(0).WithMessage("Total shipments must be greater than zero.");

        }
    }
}
