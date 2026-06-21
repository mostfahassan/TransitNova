
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Bundles.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.Bundles.Handlers.ApplyingCommands
{
    public sealed class UpdateBundleHandler(
    IGenericRepository<Bundle, Guid> repository,
    IOperationManagerQueryRepository opQuery,
    IUnitOfWork unitOfWork,
    ICacheService cacheService,
    ILogger<UpdateBundleHandler> logger)
    : ICommandHandler<UpdateBundleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateBundleCommand request, CancellationToken ct)
        {
            var operationManagerId= await opQuery.GetUserIdAsync(request.AppUserId, ct);
            var entity = await repository.GetByIdAsync<Bundle>(request.Dto.BundleId, ct) ?? null ;
            if (entity == null)
            {
                logger.LogWarning("Bundle not found. Id: {BundleId}", request.Dto.BundleId);
                return BaseResult.Failure(Errors.BundleNotFound("Bundle not found"));
            }
            entity.Update(operationManagerId.ToString(), request.Dto.BundlePrice, request.Dto.TotalWeight, request.Dto.TotalShipments);

            repository.Update(entity);
            
            await unitOfWork.SaveChangesAsync(ct);
   
            logger.LogInformation("Bundle updated successfully. Id: {BundleId}", request.Dto.BundleId);
            await cacheService.RemoveAsync(CacheKeys.BundleList());
            await cacheService.RemoveAsync(CacheKeys.BundleById(request.Dto.BundleId));
            return BaseResult.Success();
        }
    }

}
