using FluentValidation;

namespace TransitNova.BusinessLayer.Features.Roles.Commands.CommandValidators
{
    public sealed class DeleteRoleCommandValidator : AbstractValidator<DeleteRoleCommand>
    {
        public DeleteRoleCommandValidator()
        {
            RuleFor(command => command.RoleId)
                .NotEmpty();
        }
    }
}
