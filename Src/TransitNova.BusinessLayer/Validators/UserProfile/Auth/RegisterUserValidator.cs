using FluentValidation;
using System.Text.RegularExpressions;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
namespace TransitNova.BusinessLayer.Validators.UserProfile.Auth
{
    public class RegisterUserValidator : AbstractValidator<RegisterDto>
    {
       
        public RegisterUserValidator()
        {

            // Username validation rules:
            RuleFor(x => x.UserName)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Username is required.")
                .MinimumLength(4)
                .WithMessage("Username must be at least 4 characters.")
                .MaximumLength(30)
                .WithMessage("Username must not exceed 30 characters.")
                .WithMessage("Username must start with a letter.")
                .Matches(@"^[A-Za-z][A-Za-z0-9._]*$")
                .WithMessage("Username can contain only letters, numbers, dots and underscores.")
                .Must(userName =>
                    !Regex.IsMatch(userName, @"[._]{2,}"))
                .WithMessage("Username cannot contain consecutive dots or underscores.")
                .Must(userName =>
                    !userName.EndsWith(".") &&
                    !userName.EndsWith("_"))
                .WithMessage("Username cannot end with dot or underscore.")
                .Must(BeValidUserName)
                .WithMessage("Username is reserved or invalid.");



            // Email validation rules:
            RuleFor(u => u.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage("Please enter a valid email address.");



            // Password validation rules:
            RuleFor(x => x.Password)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage("Password is required.")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters.")
                .MaximumLength(64)
                .WithMessage("Password must not exceed 64 characters.")         
                .Matches(@"[A-Z]")
                .WithMessage("Password must contain at least one uppercase letter.")
                .Matches(@"[a-z]")
                .WithMessage("Password must contain at least one lowercase letter.")
                .Matches(@"\d")
                .WithMessage("Password must contain at least one number.")
                .Matches(@"[\W_]")
                .WithMessage("Password must contain at least one special character.")
                .Matches(@"^\S+$")
                .WithMessage("Password cannot contain spaces.")
                .Must(password =>
                    !Regex.IsMatch(password, @"(.)\1\1"))
                .WithMessage("Password cannot contain repeated characters.")
                .Must(BeStrongPassword)
                .WithMessage("Password is too common or weak.");




            // Confirm Password validation rules:
            RuleFor(u => u.ConfirmPassword)
                .NotEmpty()
                .Equal(u => u.Password)
                .WithMessage("Confirm Password must match the Password.");


            // Phone Number validation rules:
            RuleFor(u => u.PhoneNumber)
                .NotEmpty()
                .MaximumLength(15)
                .Matches(@"^\+?[1-9]\d{1,14}$")
                .WithMessage("Please enter a valid phone number.");

            // First Name and Last Name validation rules:
            RuleFor(u => u.FirstName)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(50)
                .Matches(@"^(?!.*\d).+$")
                .WithMessage("First Name can only contain letters and spaces.");

            RuleFor(x => x.LastName)
                .NotEmpty()
                .MinimumLength(2)
                .MaximumLength(50)
                .Matches(@"^(?!.*\d).+$")
                .WithMessage("Last Name can only contain letters and spaces.");



            // CityId validation rules:
            RuleFor(u => u.CityId)
                .NotEmpty()
                .WithMessage("Please select a valid City.");
        }
        private bool BeStrongPassword(string password)
        {
            string[] weakPasswords =
            {
            "Password123!",
            "Admin123!",
            "Qwerty123!",
            "12345678",
            "Aa123456",
            "Welcome123!"
        };

            return !weakPasswords
                .Any(x => x.Equals(password,
                    StringComparison.OrdinalIgnoreCase));
        }



        private bool BeValidUserName(string userName)
        {
            string[] reservedUserNames =
            {
        "admin",
        "administrator",
        "root",
        "system",
        "support",
        "owner",
        "superadmin",
        "moderator",
        "api",
        "test"
    };

            return !reservedUserNames
                .Any(x => x.Equals(userName,
                    StringComparison.OrdinalIgnoreCase));
        }
    }
}
