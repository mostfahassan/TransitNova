using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Bundles;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler.Bundles
{
    public sealed class UnsubscribeFromBundleHandler(
        IGenericRepository<Bundle, Guid> bundleRepository,
        IUserQueryRepository userRepository,
        IUnitOfWork unitOfWork,
        ILogger<UnsubscribeFromBundleHandler> logger)
        : ICommandHandler<UnsubscribeFromBundleCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(UnsubscribeFromBundleCommand request, CancellationToken cancellationToken)
        {
            //====== Verify User Exists ======
            var profileId = await userRepository.GetAppUserIdAsync(request.UserId, cancellationToken);
            if (profileId == Guid.Empty)
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
            var activeSubscription = bundle.Subscriptions.FirstOrDefault(s => s.SubscribedUserId == profileId && s.IsActive);
            if (activeSubscription is null)
            {
                logger.LogWarning("No active subscription found for User {UserId} to Bundle {BundleId}", request.UserId, request.BundleId);
                return BaseResult.NotFound(Errors.FailedOperation($"User has no active subscription to Bundle {request.BundleId}"));
            }

            bundle.Unsubscribe(profileId);
            
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("User {UserId} successfully unsubscribed from Bundle {BundleId}", request.UserId, request.BundleId);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Users.Profile(request.UserId),
                CacheKeys.Users.AdminDetails(request.UserId),
                CacheKeys.Bundles.List,
                CacheKeys.Bundles.ById(request.BundleId));
            return BaseResult.Success();

        }
    }
}


