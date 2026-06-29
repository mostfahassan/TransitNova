
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.IdentityOperationService;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler.Shipment
{
    public class CreateShipmentCommandHandler(
     IShipmentQueryRepository shipmentQuery,
     IShipmentService shipmentService,
     IUserAuthQueryService userQuery,
     ISystemLogCommands systemLogCommands,
     IUnitOfWork unitOfWork,
     ILogger<CreateShipmentCommandHandler> logger)
     : ICommandHandler<CreateShipmentCommand, Result<RetrieveShipmentDto>>
    {
        public async Task<Result<RetrieveShipmentDto>> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {

            logger.LogInformation("Starting shipment creation for User {SenderId}", request.AppUserId);
            var createdShipmentId = await shipmentService.PrepareShipmentCreationAsync(request.Dto,request.AppUserId, cancellationToken);

            // ===== Attempt To Retrieve the created shipment with details for response ======== //
            var detailedShipment = await shipmentQuery.CreateShipmentForUserAsync(createdShipmentId, cancellationToken);

            //===== Validate Retrieval =========//
            if (detailedShipment == null)
            {
                logger.LogWarning("Shipment created (Id: {ShipmentId}) but retrieval failed", createdShipmentId);
                return Result<RetrieveShipmentDto>.Failure(Errors.ShipmentNotFound("Shipment created but retrieval failed"));
            }

            var performedByName = (await userQuery.FindByIdAsync(request.AppUserId, cancellationToken))!.FullName!;
            var log = SystemActivityLog.AddLog(
                ActivityAction.Created,
                ActivityEntityType.Shipment,
                $"Shipment {detailedShipment.Id} with tracking number {detailedShipment.TrackingNumber} was created.",
                request.AppUserId,
                performedByName);

            await systemLogCommands.LogAsync(log, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Shipment created successfully. TrackingNumber: {TrackingNumber}, " + "ShipmentId: {ShipmentId}, Cost: {Cost}", detailedShipment.TrackingNumber,
                                          detailedShipment.Id, detailedShipment.ShippingCost);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Users.Dashboard(request.AppUserId),
                CacheKeys.Users.Profile(request.AppUserId),
                CacheKeys.Users.AdminDetails(request.AppUserId),
                CacheKeys.Users.Shipment(request.AppUserId, detailedShipment.Id),
                CacheKeys.Shipments.ByTrackingNumber(detailedShipment.TrackingNumber),
                CacheKeys.OperationManagers.Dashboard);

            //====== Return Created Result with detailed shipment data ======//
            return Result<RetrieveShipmentDto>.Created(detailedShipment);
        }
    }
}


