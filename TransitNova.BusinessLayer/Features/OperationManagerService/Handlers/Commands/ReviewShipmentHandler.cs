using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.OperationManagerService.Commands;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.Commands
{
    public sealed class ReviewShipmentHandler(
        IShipmentQueryRepository shipmentQueryRepo)

        : ICommandHandler<ReviewShipmentCommand, Result<RetrieveShipmentDto>>
    {
        public async Task<Result<RetrieveShipmentDto>> Handle(ReviewShipmentCommand request, CancellationToken cancellationToken)
        {
            // Get the shipment details to be reviewed
            var shipmentToReview = await shipmentQueryRepo.CreateShipmentForUserAsync(request.ShipmentId, cancellationToken);
            if (shipmentToReview == null)
            {
                return Result<RetrieveShipmentDto>.Failure(Errors.ShipmentNotFound("Shipment Not Found"));
            }
            return Result<RetrieveShipmentDto>.Success(shipmentToReview);
        }
    }
}
