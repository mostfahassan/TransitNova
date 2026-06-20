
using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Shipment;

namespace TransitNova.BusinessLayer.Features.Shipments.Commands.CommandValidators
{
    public sealed class ShipmentFiltrationCommandValidator : AbstractValidator<ShipmentFiltrationCommand>
    {

        public ShipmentFiltrationCommandValidator(IValidator<ShipmentFilterDto> dto)
        {

            RuleFor(x => x.FilterCriteria)
                .SetValidator(dto);
        }










    }
}
