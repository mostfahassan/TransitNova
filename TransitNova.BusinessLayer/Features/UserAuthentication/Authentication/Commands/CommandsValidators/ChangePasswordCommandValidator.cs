using FluentValidation;
using TransitNova.BusinessLayer.DTOs.UserProfile.Auth;
using TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.UserAuthentication.Authentication.Commands.CommandsValidators
{
    public sealed class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
    {
        public ChangePasswordCommandValidator(IValidator<ChangePasswordDto> dtoValidator)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.AppUserId)
                .NotEmpty()
                .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");



        }
    }
}
