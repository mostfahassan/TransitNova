
using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Common.ResultPattern;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands.Checking
{
    public sealed class IsCarrierAvailableQueryHandler(
        ICarrierRulesRepository carrierQueryRepo,
        ILogger<IsCarrierAvailableQueryHandler> logger)
        : ICommandHandler<CarrierAvailabilityCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(CarrierAvailabilityCommand request, CancellationToken ct)
        {
            logger.LogDebug("Checking availability for Carrier {UserId}", request.CarrierId);

            //======= Check Carrier Avilabilty 
            var available = await carrierQueryRepo.IsCarrierAvailableForAssignmentAsync(request.CarrierId, ct);
            logger.LogInformation("Carrier {UserId} availability: {IsAvailable}", request.CarrierId, available);
            return available
                ? BaseResult.Success()
                : BaseResult.Conflict(Errors.CarrierNotAvailable("Carrier is not available right now"));
        }
    }
}