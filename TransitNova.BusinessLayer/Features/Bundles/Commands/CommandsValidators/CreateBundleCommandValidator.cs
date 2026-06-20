using System.Security.Claims;
using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Bundle;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.BusinessLayer.Features.Bundles.Commands.CommandsValidators
{
    public sealed class CreateBundleCommandValidator : AbstractValidator<CreateBundleCommand>
    {
        public CreateBundleCommandValidator(IValidator<CreateBundleDto> dtoValidator)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .SetValidator(dtoValidator);

            RuleFor(x => x.UserId)
                 .NotEmpty()
                .WithMessage($"{ErrorCode.REQUIRED_FIELD}");
        }
    }
}
