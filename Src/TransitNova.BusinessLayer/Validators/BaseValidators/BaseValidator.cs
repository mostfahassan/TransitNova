using FluentValidation;
using TransitNova.BusinessLayer.Common.CommonData;

namespace TransitNova.BusinessLayer.Validators.BaseValidators
{
    public class BaseValidator<T>:AbstractValidator<T> where T : CommonUpdateData
    {
        public BaseValidator()
        {

            RuleFor(u => u.FirstName)
                .NotEmpty()
                .MaximumLength(50)
                .MinimumLength(2)
                .Matches(@"^[\p{L}\s]+$")
                .WithMessage("First Name must be between 2 and 50 characters long.");

            RuleFor(u => u.LastName)
                .NotEmpty()
                .MaximumLength(50)
                .MinimumLength(2)
                .Matches(@"^[\p{L}\s]+$")
                .WithMessage("Last Name must be between 2 and 50 characters long.");

            RuleFor(u => u.PhoneNumber)
                .NotEmpty()
                .MaximumLength(15)
                .MinimumLength(7)
                .WithMessage("Phone Number must be between 7 and 15 characters long");

            RuleFor(u => u.CityId)
                .NotEmpty()
                .WithMessage("Must Enter Your City");

            RuleFor(u => u.Address)
                .MaximumLength(100)
                .WithMessage("Address must be less than 100 characters long");
        }
    }
    }

