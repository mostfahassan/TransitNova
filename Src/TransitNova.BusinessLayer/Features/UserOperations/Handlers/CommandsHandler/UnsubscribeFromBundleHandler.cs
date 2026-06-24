using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler
{
    public sealed class UnsubscribeFromBundleHandler(
        IGenericRepository<Bundle, Guid> bundleRepository,
        IUserQueryRepository userRepository,
        IUnitOfWork unitOfWork,
        ICacheService cacheService,
        ILogger<UnsubscribeFromBundleHandler> logger)
        : ICommandHandler<UnsubscribeFromBundleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UnsubscribeFromBundleCommand request, CancellationToken cancellationToken)
        {
            //====== Verify User Exists ======
            var userId = await userRepository.GetAppUserIdAsync(request.UserId, cancellationToken);
            if (userId == Guid.Empty)
            {
                logger.LogWarning("User with ID {UserId} not found for bundle unsubscription", request.UserId);
                return BaseResult.NotFound(Errors.UserNotFound($"User with ID {request.UserId} not found"));
            }

            //====== Retrieve Bundle ======
            var bundle = await bundleRepository.GetByIdAsync<Bundle>(request.BundleId, cancellationToken);
            if (bundle is null)
            {
                logger.LogWarning("Bundle with ID {BundleId} not found for unsubscription", request.BundleId);
                return BaseResult.NotFound(Errors.NotFound($"Bundle with ID {request.BundleId} not found"));
            }

            //====== Check Active Subscription Exists ======
            var activeSubscription = bundle.Subscriptions.FirstOrDefault(s => s.SubscribedUserId == request.UserId && s.IsActive);
            if (activeSubscription is null)
            {
                logger.LogWarning("No active subscription found for User {UserId} to Bundle {BundleId}", request.UserId, request.BundleId);
                return BaseResult.NotFound(Errors.FailedOperation($"User has no active subscription to Bundle {request.BundleId}"));
            }

            bundle.Unsubscribe(request.UserId);
            
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User {UserId} successfully unsubscribed from Bundle {BundleId}", request.UserId, request.BundleId);
            await cacheService.RemoveAsync(CacheKeys.UserProfile(request.UserId));
            await cacheService.RemoveAsync(CacheKeys.AdminUserDetails(request.UserId));
            await cacheService.RemoveAsync(CacheKeys.BundleList());
            await cacheService.RemoveAsync(CacheKeys.BundleById(request.BundleId));
            return BaseResult.Success();

        }
    }
}
