
using TransitNova.BusinessLayer.Common.CQRS;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Features.Carriers.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.Domain.Contracts.Caching;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyCommands.Crud
{
    public sealed class DeleteCarrierHandler(
        ICarrierCommandRepository carrierRepository,
        ICacheService cacheService,
        ILogger<DeleteCarrierHandler> logger)
        : ICommandHandler<DeleteCarrierCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(DeleteCarrierCommand request, CancellationToken cancellationToken)
        {
            //==== Deleting Carrier Attempts ====== 
            var affectedRows = await carrierRepository.DeleteCarrierAsync(request.CarrierId ,cancellationToken);
            if (affectedRows > 0)
            {
                logger.LogInformation("Carrier {CarrierId} was deleted by administrator {AdminId}.", request.CarrierId, request.AdminId);
                await cacheService.RemoveAsync(CacheKeys.CarrierProfile(request.CarrierId));
                await cacheService.RemoveAsync(CacheKeys.CarrierDashboard(request.CarrierId));
                await cacheService.RemoveAsync(CacheKeys.CarrierTrips(request.CarrierId));
                return BaseResult.Success();
            }
            return BaseResult.Failure(Errors.FailedOperation("Carrier Deletion Failed"));
        }
    }
}
