using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Carriers;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.CommandValidators
{
    public sealed class GetCarriersForOperationManagerCommandValidator : AbstractValidator<GetCarriersForOperationManagerQuery>
    {
        public GetCarriersForOperationManagerCommandValidator(IValidator<FilterCarrierDto> dto)

        {
            RuleFor(x => x.FilterCriteria)
            .SetValidator(dto);
        
        }
    }

}
