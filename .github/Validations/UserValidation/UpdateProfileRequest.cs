using FluentValidation;
using TransitNova.Application.DTO_s.UserProfileDto.IdentityDto;

namespace TransitNova.Application.Validations.UserValidation
{
    internal sealed class UpdateProfileRequest : AbstractValidator<UpdateProfileRequestDto>
    {
        /*
      string FirstName,
      string LastName,
      string? PhoneNumber,
      int CityId,
      int CountryId,
      string? Address
        */
        public UpdateProfileRequest()
        {

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

            RuleFor(u => u.PhoneNumber)
                .NotEmpty()
                .MaximumLength(15)
                .MinimumLength(7)
                .WithMessage("Phone Number must be between 7 and 15 characters long");

            RuleFor(u => u.CityId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("City Id must be greater than 0");

            RuleFor(u => u.CountryId)
                .NotEmpty()
                .GreaterThan(0)
                .WithMessage("City Id must be greater than 0");

            RuleFor(u => u.Address)
                .NotEmpty()
                .MaximumLength(100)
                .WithMessage("Address must be less than 100 characters long");
        }
    }
}
