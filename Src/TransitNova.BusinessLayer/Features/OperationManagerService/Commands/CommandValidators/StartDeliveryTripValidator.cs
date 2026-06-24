using FluentValidation;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Trips;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Commands.CommandValidators
{
    public sealed class StartDeliveryTripValidator : AbstractValidator<StartDeliveryTripCommand>
    {
        public StartDeliveryTripValidator(
            IOperationManagerRulesRepository managerRepository,
            ICarrierRulesRepository carrierRepository)
        {
            RuleFor(x => x.OperationManagerId)
            .NotEmpty()
            .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

            RuleFor(x => x.CarrierId)
                .NotEmpty()
              .WithErrorCode($"{ErrorCode.UNAUTHORIZED}");

        }
    }

}
