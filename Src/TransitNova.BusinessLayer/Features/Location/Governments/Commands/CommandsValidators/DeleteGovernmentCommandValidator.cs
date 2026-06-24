using FluentValidation;

namespace TransitNova.BusinessLayer.Features.Location.Governments.Commands.CommandsValidators
{
    public sealed class DeleteGovernmentCommandValidator : AbstractValidator<DeleteGovernmentCommand>
    {
        public DeleteGovernmentCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0);
        }
    }
}
