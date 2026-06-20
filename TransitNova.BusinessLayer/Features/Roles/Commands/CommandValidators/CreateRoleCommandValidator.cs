using FluentValidation;

namespace TransitNova.BusinessLayer.Features.Roles.Commands.CommandValidators
{
    public sealed class CreateRoleCommandValidator : AbstractValidator<CreateRoleCommand>
    {
        public CreateRoleCommandValidator()
        {
            RuleFor(command => command.RoleName)
                .NotEmpty()
                .MaximumLength(256);
        }
    }
}
