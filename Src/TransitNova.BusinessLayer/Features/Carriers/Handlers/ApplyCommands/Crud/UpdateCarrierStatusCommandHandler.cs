
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands.Crud
{
    public sealed class UpdateCarrierStatusCommandHandler(
    ICarrierCommandRepository carrierRepo,
    ILogger<UpdateCarrierStatusCommandHandler> logger)
    : ICommandHandler<UpdateCarrierStatusCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateCarrierStatusCommand request, CancellationToken ct)
        {
            var affectedRows = await carrierRepo.UpdateStatusAsync(request.CarrierId, request.Status, ct);
            if (affectedRows > 0)
            {
                logger.LogInformation("Carrier {UserId} status updated to {Status}", request.CarrierId, request.Status);
                CacheInvalidationContext.Set(
                    request,
                    CacheKeys.Carriers.Profile(request.CarrierId),
                    CacheKeys.Carriers.Dashboard(request.CarrierId),
                    CacheKeys.Carriers.ByStatus(request.Status));
                return BaseResult.Success();
            }
            logger.LogWarning("Failed to update status for Carrier {UserId}", request.CarrierId);
            return BaseResult.Failure(Errors.CarrierNotFound("Carrier Not Found"));
        }
    }
}


