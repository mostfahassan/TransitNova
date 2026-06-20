using FluentValidation;

using TransitNova.Application.DTO_s.UserProfileDto.IdentityDto;

namespace TransitNova.Application.Validations.UserValidation
{
    internal sealed class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        /*
        string UserName,
        string Email,
        string Password,
        string ConfirmPassword,
        string Phone,
        string FirstName,
        string LastName,
        int CountryId,
        int GovernmentId,
        int CityId
            */
        public RegisterUserValidator()
        {
            RuleFor(u => u.UserName).
                NotEmpty()
                .MinimumLength(3)
                .MaximumLength(10)
                .Matches(@"^[a-zA-Z0-9#@$%^&*!]+$")
                .WithMessage("Username must be 6-10 characters long and contain only letters and numbers.");


            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Please enter a valid email address.");


            RuleFor(u => u.Password).NotEmpty()
                .MinimumLength(8)
                .MaximumLength(20)
                .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[#@$%^&*!]).+$")
                .WithMessage("Password must be 8-20 characters long and contain at least one uppercase letter, one lowercase letter, one digit, and one special character.");

            RuleFor(u => u.ConfirmPassword)
                .NotEmpty()
                .Equal(u => u.Password)
                .WithMessage("Confirm Password must match the Password.");

            RuleFor(u => u.Phone)
                .NotEmpty()
                .MaximumLength(15)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Please enter a valid phone number.");


            RuleFor(u => u.FirstName)
                .NotEmpty()
                .MaximumLength(10)
                .MinimumLength(2)
                .Matches(@"^[\p{L}\s]+$")
                .WithMessage("First Name must be between 2 and 10 characters long.");

            RuleFor(u => u.LastName)
                .NotEmpty()
                .MaximumLength(10)
                .MinimumLength(2)
                .Matches(@"^[\p{L}\s]+$")
                .WithMessage("Last Name must be between 2 and 10 characters long.");


            RuleFor(u => u.CountryId)
                .GreaterThan(0)
                .WithMessage("Please select a valid country.");


            RuleFor(u => u.CityId)
                .GreaterThan(0)
                .WithMessage("Please select a valid City.");

            RuleFor(u => u.GovernmentId)
                .GreaterThan(0)
                .WithMessage("Please select a valid government.");
        }

    }
}
