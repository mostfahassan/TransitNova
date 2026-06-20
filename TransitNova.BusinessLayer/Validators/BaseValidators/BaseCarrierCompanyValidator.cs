using FluentValidation;
using TransitNova.BusinessLayer.Common.CommonData;
namespace TransitNova.BusinessLayer.Validators.BaseValidators
{
    public class BaseCarrierCompanyValidator <T>:AbstractValidator<T> where T : BaseCarrierCompanyData
    {
        public BaseCarrierCompanyValidator()
        {
            When(x => x.Name != null, () => {

                RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Company name is required.")
               .MaximumLength(100).WithMessage("Company name must not exceed 100 characters.");

            });
            When(x => x.Email != null, () =>
            {
                RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Please enter a valid email address.");
            });
            When(x => x.PhoneNumber != null, () =>
            {
                RuleFor(x => x.PhoneNumber)
                     .NotEmpty().WithMessage("Phone number is required.")
                     .MaximumLength(15).WithMessage("Phone number must not exceed 15 characters.");
            });
            When(x => x.Address != null, () =>
            {
                RuleFor(x => x.Address)
               .NotEmpty().WithMessage("Address is required.")
               .MaximumLength(500).WithMessage("Address must not exceed 500 characters.");
            });
        }
    }
}
