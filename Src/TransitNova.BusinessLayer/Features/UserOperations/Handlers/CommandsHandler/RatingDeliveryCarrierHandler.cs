using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserOperations.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRatingRepository;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler
{
    public class RatingDeliveryCarrierHandler(
            IShipmentRulesRepository shipmentRepo,
            ICarrierQueryRepository carrierQuery,
            ICarrierRatingCommandsRepository ratingRepo,
            IUnitOfWork unitOfWork,
            ICacheService cacheService,
            ILogger<RatingDeliveryCarrierHandler> logger)
          : ICommandHandler<RateDeliveryCarrierCommand, BaseResult>
    {
        public async Task<BaseResult> Handle(RateDeliveryCarrierCommand request, CancellationToken cancellationToken)
        {
            // ==== Check If Carrier Related To Sender And Carrier Delivered The Shipment
            var canRate = await shipmentRepo.CanRateDeliveryCarrierAsync(request.shipmentId, request.Dto.CarrierId, request.AppUserId, cancellationToken);
            if (!canRate)
            {
                logger.LogWarning("User {UserId} attempted to rate delivery carrier {CarrierId} for shipment {ShipmentId} but validation failed",
                      request.AppUserId,
                      request.Dto.CarrierId,
                      request.shipmentId);

                return BaseResult.Failure(Errors.FailedOperation("User Cant Rate The Shipment "));
            }


            // === Retrieve Carrier For Rating 
            var carrier = await carrierQuery.GetCarrierAsync(c => c.Id == request.Dto.CarrierId, cancellationToken);
            if (carrier == null)
            {
                logger.LogWarning("Delivery carrier {CarrierId} was not found while user {UserId} attempted to submit a rating for shipment {ShipmentId}",
                       request.Dto.CarrierId,
                       request.AppUserId,
                       request.shipmentId);


                return BaseResult.NotFound(Errors.CarrierNotFound("Rated Carrier Not Found"));
            }

            // === Add Rate To Carrier
            carrier.AddRating(request.Dto.Rating);

            // === Creating Rating Record 
            var rating = CarrierRating.Create(request.Dto.CarrierId, request.shipmentId, request.AppUserId, request.Dto.Rating, request.Dto.Comment);
            await ratingRepo.AddRatingAsync(rating, cancellationToken);

            await unitOfWork.SaveChangesAsync(cancellationToken);
 
            logger.LogInformation("User {UserId} rated delivery carrier {CarrierId} with rating {Rating} for shipment {ShipmentId}", request.AppUserId,
                  request.Dto.CarrierId,
                  request.Dto.Rating,
                  request.shipmentId);

            await cacheService.RemoveAsync(CacheKeys.CarrierProfile(request.Dto.CarrierId));
            await cacheService.RemoveAsync(CacheKeys.CarrierDashboard(request.Dto.CarrierId));
            await cacheService.RemoveAsync(CacheKeys.CarrierRating(request.Dto.CarrierId));
            return BaseResult.Success();

        }
    }
}
