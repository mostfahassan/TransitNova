using FluentValidation;

namespace TransitNova.BusinessLayer.Features.Location.Governments.Commands.CommandsValidators
{
    public sealed class UpdateGovernmentCommandValidator : AbstractValidator<UpdateGovernmentCommand>
    {
        public UpdateGovernmentCommandValidator()
        {
            RuleFor(command => command.Id)
                .GreaterThan(0);

            RuleFor(command => command.Name)
                .NotEmpty()
                .MaximumLength(100);

            RuleFor(command => command.CountryId)
                .GreaterThan(0);
        }
    }
}
