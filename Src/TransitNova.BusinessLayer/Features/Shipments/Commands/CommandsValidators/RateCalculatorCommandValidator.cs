using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.Shipments.Commands;

namespace TransitNova.BusinessLayer.Features.Shipments.Commands.CommandsValidators
{
    public sealed class RateCalculatorCommandValidator : AbstractValidator<RateCalculatorCommand>
    {
        public RateCalculatorCommandValidator(IValidator<RateCalculatorDto> dtoValidator)
        {
            RuleFor(x => x.Dto)
                .NotNull()
                .WithMessage("Rate calculation payload is required.")
                .SetValidator(dtoValidator);
        }
    }
}
