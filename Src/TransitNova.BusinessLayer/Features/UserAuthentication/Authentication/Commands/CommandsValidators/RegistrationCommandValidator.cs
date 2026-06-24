using FluentValidation;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;

namespace TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands.CommandsValidators
{
    public sealed class RegistrationCommandValidator : AbstractValidator<RegistrationCommand>
    {
        public RegistrationCommandValidator(IValidator<RegisterDto> dtoValidator)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);
        }
    }
}
