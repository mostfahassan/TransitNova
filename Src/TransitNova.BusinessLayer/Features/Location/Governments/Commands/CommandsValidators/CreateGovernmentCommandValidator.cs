using FluentValidation;

namespace TransitNova.BusinessLayer.Features.Location.Governments.Commands.CommandsValidators
{
    public sealed class CreateGovernmentCommandValidator : AbstractValidator<CreateGovernmentCommand>
    {
        public CreateGovernmentCommandValidator()
        {
            RuleFor(command => command.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(command => command.CountryId)
                .GreaterThan(0);
        }
    }
}
