using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.Token.Commands.CommandsValidator
{
    public sealed class GenerateRefreshTokenCommandValidator: AbstractValidator<GenerateRefreshTokenCommand>
    {
        public GenerateRefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty()
                .NotNull()
                .WithErrorCode($"{ErrorCode.REQUIRED_FIELD}"); 
              
        }
    }
}
