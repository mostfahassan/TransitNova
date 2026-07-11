
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.Bundles.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.Bundles.Handlers.ApplyingCommands
{
    public sealed class CreateBundleHandler(
     IGenericRepository<Bundle, Guid> repository,
     IUnitOfWork unitOfWork,
     ILogger<CreateBundleHandler> logger)
     :ICommandHandler<CreateBundleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(CreateBundleCommand request, CancellationToken ct)
        {

             var bundle = Bundle.Create(
             creatorId: request.UserId.ToString(),
             bundleName: request.Dto.BundleName,
             description: request.Dto.BundleDescription,
             bundlePrice: request.Dto.BundlePrice,
             tier: request.Dto.Tier,
             durationMonths: request.Dto.BundleDurationMonths,
             maxShipments: request.Dto.MaxShipmentsPerMonth,
             maxWeight: request.Dto.MaxWeightPerShipment,
             maxDistance: request.Dto.MaxDistancePerShipment,
             discountPercentage: request.Dto.DiscountPercentage,
             minShipmentValue: request.Dto.MinimumShipmentValueForDiscount);

            await repository.AddAsync(bundle, ct);
            await unitOfWork.SaveChangesAsync(ct);

            logger.LogInformation("Bundle created successfully. Id: {BundleId}", bundle.Id);
            CacheInvalidationContext.Set(request, CacheKeys.Bundles.List);
            return BaseResult.Created("Bundle created successfully.");
        }
    }

}


