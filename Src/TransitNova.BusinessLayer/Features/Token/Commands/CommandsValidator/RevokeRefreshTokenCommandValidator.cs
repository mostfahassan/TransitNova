using FluentValidation;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.Token.Commands.CommandsValidator
{
    public sealed class RevokeRefreshTokenCommandValidator : AbstractValidator<RevokeRefreshTokenCommand>
    {
        public RevokeRefreshTokenCommandValidator()
        {
            RuleFor(x => x.AppUserId)
                .NotEmpty()
                .NotNull()
                .WithErrorCode($"{ErrorCode.REQUIRED_FIELD}"); 
              
        }
    }
}
