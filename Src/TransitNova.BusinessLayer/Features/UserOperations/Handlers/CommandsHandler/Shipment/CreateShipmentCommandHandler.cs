
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;
using TransitNova.BusinessLayer.DTOs.Payment;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler.Shipment
{
    public class CreateShipmentCommandHandler(
     IShipmentService shipmentService,
     IUserAuthQueryService userQuery,
     ISystemLogCommands systemLogCommands,
     IUnitOfWork unitOfWork,
     ILogger<CreateShipmentCommandHandler> logger)
     : ICommandHandler<CreateShipmentCommand, Result<Invoice>>
    {
        public async Task<Result<Invoice>> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {

            logger.LogInformation("Starting shipment creation for User {SenderId}", request.AppUserId);
            var (createdShipment, trackingNumber) = await shipmentService.HandleShipmentCreation(request.Dto,request.AppUserId, cancellationToken);
            if (createdShipment == null || createdShipment.Data == null)
            {
                logger.LogError("Shipment creation failed for User {SenderId}", request.AppUserId);
                return Result<Invoice>.Failure(Errors.ShipmentCreationFailed("Failed to create shipment."));
            }
            if (string.IsNullOrWhiteSpace(trackingNumber))
            {
                logger.LogError("Failed to retrieve tracking number for created shipment.");
                return Result<Invoice>.Failure(Errors.FailedOperation("Failed to create shipment."));
            }

            var performedByName = (await userQuery.FindByIdAsync(request.AppUserId, cancellationToken))!.FullName!;
            var log = SystemActivityLog.AddLog(
                ActivityAction.Created,
                ActivityEntityType.Shipment,
                $"Shipment {createdShipment.Data!.ShipmentId} with Ship number {createdShipment.Data.ShippingCost} was created.",
                request.AppUserId,
                performedByName);

            await systemLogCommands.LogAsync(log, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Shipment created successfully. TrackingNumber: {TrackingNumber}, " + "ShipmentId: {ShipmentId}, Cost: {Cost}", trackingNumber,
                                          createdShipment.Data.ShipmentId, createdShipment.Data.ShippingCost);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Users.Dashboard(request.AppUserId),
                CacheKeys.Users.Profile(request.AppUserId),
                CacheKeys.Users.AdminDetails(request.AppUserId),
                CacheKeys.Users.Shipment(request.AppUserId, createdShipment.Data.ShipmentId),
                CacheKeys.Shipments.ByTrackingNumber(trackingNumber),
                CacheKeys.OperationManagers.OperationManagersDashboard);

            //====== Return Created Result detailed Invoice ======//
            return Result<Invoice>.Created(createdShipment.Data);
        }
    }
}


