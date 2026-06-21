
using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands
{
    public sealed class UpdateCarrierStatusCommandHandler(
    ICarrierCommandRepository carrierRepo,
    ICacheService cacheService,
    ILogger<UpdateCarrierStatusCommandHandler> logger)
    : ICommandHandler<UpdateCarrierStatusCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateCarrierStatusCommand request, CancellationToken ct)
        {
            var affectedRows = await carrierRepo.UpdateStatusAsync(request.CarrierId, request.Status, ct);
            if (affectedRows > 0)
            {
                logger.LogInformation("Carrier {UserId} status updated to {Status}", request.CarrierId, request.Status);
                await cacheService.RemoveAsync(CacheKeys.CarrierProfile(request.CarrierId));
                await cacheService.RemoveAsync(CacheKeys.CarrierDashboard(request.CarrierId));
                await cacheService.RemoveAsync(CacheKeys.CarriersByStatus(request.Status));
                return BaseResult.Success();
            }
            logger.LogWarning("Failed to update status for Carrier {UserId}", request.CarrierId);
            return BaseResult.Failure(Errors.CarrierNotFound("Carrier Not Found"));
        }
    }
}
