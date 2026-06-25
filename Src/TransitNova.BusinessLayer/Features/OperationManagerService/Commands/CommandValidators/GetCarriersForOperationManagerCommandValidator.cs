using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Trips;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.Carriers;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Result;
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
