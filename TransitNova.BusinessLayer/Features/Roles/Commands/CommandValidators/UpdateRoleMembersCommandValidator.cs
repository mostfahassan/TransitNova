using FluentValidation;

namespace TransitNova.BusinessLayer.Features.Roles.Commands.CommandValidators
{
    public sealed class UpdateRoleMembersCommandValidator : AbstractValidator<UpdateRoleMembersCommand>
    {
        public UpdateRoleMembersCommandValidator()
        {
            RuleFor(command => command.RoleId)
                .NotEmpty();

            RuleFor(command => command.Users)
                .NotNull();

            RuleForEach(command => command.Users)
                .ChildRules(user =>
                {
                    user.RuleFor(member => member.UserId)
                        .NotEmpty();
                });
        }
    }
}
