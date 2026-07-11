
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Features.UserOperations.Commands.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.SystemLogRepository;
using TransitNova.BusinessLayer.Interfaces.Services.ShipmentServices;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.SystemLogs;
using TransitNova.BusinessLayer.DTOs.Payment;
using TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository;
namespace TransitNova.BusinessLayer.Features.UserOperations.Handlers.CommandsHandler.Shipment
{
    public class CreateShipmentCommandHandler(
     IShipmentService shipmentService,
     IUserQueryRepository userQuery,
     ISystemLogCommands systemLogCommands,
     IUnitOfWork unitOfWork,
     ILogger<CreateShipmentCommandHandler> logger)
     : ICommandHandler<CreateShipmentCommand, Result<ShipmentPaymentInvoiceDto>>
    {
        public async Task<Result<ShipmentPaymentInvoiceDto>> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {

            logger.LogInformation("Starting shipment creation for User {SenderId}", request.AppUserId);
            var (createdShipment, trackingNumber) = await shipmentService.HandleShipmentCreation(request.Dto,request.AppUserId, cancellationToken);
            
            
            if (createdShipment == null || createdShipment.Data == null || string.IsNullOrWhiteSpace(trackingNumber))
            {
                logger.LogError("Shipment creation failed for User {SenderId}", request.AppUserId);
                return Result<ShipmentPaymentInvoiceDto>.Failure(Errors.ShipmentCreationFailed("Failed to create shipment."));
            }

            var performedByName = !string.IsNullOrWhiteSpace(await userQuery.GetUserFullName(request.AppUserId, cancellationToken))
                ? await userQuery.GetUserFullName(request.AppUserId, cancellationToken)
                : request.AppUserId.ToString();


            var log = SystemActivityLog.AddLog(
                ActivityAction.Created,
                ActivityEntityType.Shipment,
                $"Shipment {createdShipment.Data!.ReferenceId} with Ship number {createdShipment.Data.Amount} was created.",
                request.AppUserId,
                performedByName!);

            await systemLogCommands.LogAsync(log, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            logger.LogInformation("Shipment created successfully. TrackingNumber: {TrackingNumber}, " + "ShipmentId: {ShipmentId}, Cost: {Cost}", trackingNumber,
                                          createdShipment.Data.ReferenceId, createdShipment.Data.Amount);
            CacheInvalidationContext.Set(
                request,
                CacheKeys.Users.Dashboard(request.AppUserId),
                CacheKeys.Users.Profile(request.AppUserId),
                CacheKeys.Users.AdminDetails(request.AppUserId),
                CacheKeys.Users.Shipment(request.AppUserId, createdShipment.Data.ReferenceId),
                CacheKeys.Shipments.ByTrackingNumber(trackingNumber),
                CacheKeys.OperationManagers.OperationManagersDashboard);

            //====== Return Created Result detailed InvoiceReport ======//
            var paymentInvoiceDto = new ShipmentPaymentInvoiceDto
            {
                InvoiceId = $"INV-{createdShipment.Data.PaymentId.ToString()[..8]}",
                PaymentId = createdShipment.Data.PaymentId,
                ReferenceId = createdShipment.Data.ReferenceId,
                ReferenceType = createdShipment.Data.ReferenceType ?? "Shipment",
                ShipmentTrackingNumber = trackingNumber,
                CustomerName = performedByName,
                ShippingCost = createdShipment.Data.Amount,
                ShipmentId = createdShipment.Data.ReferenceId,
                Commission = createdShipment.Data.Commission,
                TotalAmount = createdShipment.Data.TotalAmount,
                PaymentMethod = createdShipment.Data.PaymentMethod,
                Status = createdShipment.Data.Status,
                PaidAt = createdShipment.Data.PaidAt,
                Currency = request.Dto.Currency,
                Notes = createdShipment.Data.Notes
            };
            return Result<ShipmentPaymentInvoiceDto>.Created(paymentInvoiceDto);
        }
    }
}





