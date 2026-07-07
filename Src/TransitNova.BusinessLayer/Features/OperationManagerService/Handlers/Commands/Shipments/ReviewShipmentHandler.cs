using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands.Shipments;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.Domain.Enums.Shipment;

namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands.Shipments
{
    public sealed class ReviewShipmentHandler(
        IShipmentQueryRepository shipmentQueryRepo,
        IOperationManagerQueryRepository operationManagerRepository)
        : ICommandHandler<ReviewShipmentCommand, Result<RetrieveShipmentDto>>
    {
        public async Task<Result<RetrieveShipmentDto>> Handle(ReviewShipmentCommand request, CancellationToken cancellationToken)
        {
            var operationManagerId = await operationManagerRepository.GetUserIdAsync(request.OperationManagerId, cancellationToken);
            if (operationManagerId == Guid.Empty)
            {
                return Result<RetrieveShipmentDto>.Forbidden(Errors.Forbidden("Operation manager profile was not found."));
            }

            var shipmentToReview = await shipmentQueryRepo.GetShipmentAsync(
                sh => sh.Id == request.ShipmentId && sh.CurrentStatus == ShipmentStatuses.Pending,
                cancellationToken);

            if (shipmentToReview == null)
            {
                return Result<RetrieveShipmentDto>.NotFound(Errors.ShipmentNotFound("Shipment not found or is no longer pending review."));
            }

            return Result<RetrieveShipmentDto>.Success(shipmentToReview);
        }
    }
}