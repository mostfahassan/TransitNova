using FluentValidation;

namespace TransitNova.BusinessLayer.Features.Roles.Commands.CommandValidators
{
    public sealed class UpdateRoleCommandValidator : AbstractValidator<UpdateRoleCommand>
    {
        public UpdateRoleCommandValidator()
        {
            RuleFor(command => command.RoleId)
                .NotEmpty();

            RuleFor(command => command.RoleName)
                .NotEmpty()
                .MaximumLength(256);
        }
    }
}
