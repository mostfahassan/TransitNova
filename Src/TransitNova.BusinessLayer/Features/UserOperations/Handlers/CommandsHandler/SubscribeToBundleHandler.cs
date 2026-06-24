using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler
{
    public sealed class SubscribeToBundleHandler(
        IGenericRepository<Bundle, Guid> bundleRepository,
        IUserQueryRepository userRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<SubscribeToBundleHandler> logger)
        : ICommandHandler<SubscribeToBundleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(SubscribeToBundleCommand request, CancellationToken cancellationToken)
        {
            //====== Verify User Exists ======
            var userId = await userRepository.GetAppUserIdAsync(request.UserId, cancellationToken);
            if (userId == Guid.Empty)
            {
                logger.LogWarning("User with ID {UserId} not found for bundle subscription", request.UserId);
                return BaseResult.NotFound(Errors.UserNotFound($"User with ID {request.UserId} not found"));
            }

            //====== Retrieve Bundle ======
            var bundle = await bundleRepository.GetByIdAsync<Bundle>(request.BundleId, cancellationToken);
            if (bundle is null)
            {
                logger.LogWarning("Bundle with ID {BundleId} not found for subscription", request.BundleId);
                return BaseResult.NotFound(Errors.NotFound($"Bundle with ID {request.BundleId} not found"));
            }

            //====== Subscribe User to Bundle ======

            bundle.Subscribe(request.UserId);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User {UserId} successfully subscribed to Bundle {BundleId}", request.UserId, request.BundleId);
            await cacheService.RemoveAsync(CacheKeys.UserProfile(request.UserId));
            await cacheService.RemoveAsync(CacheKeys.AdminUserDetails(request.UserId));
            await cacheService.RemoveAsync(CacheKeys.BundleList());
            await cacheService.RemoveAsync(CacheKeys.BundleById(request.BundleId));
            return BaseResult.Success();
        }


    }
}

