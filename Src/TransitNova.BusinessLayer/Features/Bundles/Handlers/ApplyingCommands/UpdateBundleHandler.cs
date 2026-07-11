
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Bundles.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.Bundles.Handlers.ApplyingCommands
{
    public sealed class UpdateBundleHandler(
    IGenericRepository<Bundle, Guid> repository,
    IOperationManagerQueryRepository opQuery,
    IUnitOfWork unitOfWork,
    ILogger<UpdateBundleHandler> logger)
    : ICommandHandler<UpdateBundleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UpdateBundleCommand request, CancellationToken ct)
        {
            var operationManagerId= await opQuery.GetUserIdAsync(request.AppUserId, ct);
            var entity = await repository.GetByIdAsync<Bundle>(request.BundleId, ct) ?? null ;
            if (entity == null)
            {
                logger.LogWarning("Bundle not found. Id: {BundleId}", request.BundleId);
                return BaseResult.Failure(Errors.BundleNotFound("Bundle not found"));
            }

            entity.Update(
                userId: operationManagerId.ToString(),
                bundlePrice: request.Dto.BundlePrice,
                description: request.Dto.BundleDescription,
                discountPercentage: request.Dto.DiscountPercentage,
                minShipmentValue: request.Dto.MinimumShipmentValueForDiscount);

            repository.Update(entity);
            
            await unitOfWork.SaveChangesAsync(ct);
   
            logger.LogInformation("Bundle updated successfully. Id: {BundleId}", request.BundleId);
            CacheInvalidationContext.Set(request, CacheKeys.Bundles.List, CacheKeys.Bundles.ById(request.BundleId));
            return BaseResult.Success();
        }
    }

}


