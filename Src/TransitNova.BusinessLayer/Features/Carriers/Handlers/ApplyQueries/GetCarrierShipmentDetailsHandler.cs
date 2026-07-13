using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries
{
    public sealed class GetCarrierShipmentDetailsHandler(
        ICarrierShipmentQueryRepository carrierShipmentRepository,
        ILogger<GetCarrierShipmentDetailsHandler> logger)
        : IQueryHandler<GetCarrierShipmentDetailsQuery, Result<RetrieveShipmentDto>>
    {
        public async Task<Result<RetrieveShipmentDto>> Handle(GetCarrierShipmentDetailsQuery request, CancellationToken ct)
        {
            var shipment = await carrierShipmentRepository.GetCarrierShipmentAsync(request.CarrierId, request.ShipmentId, ct);
            if (shipment is null)
            {
                logger.LogWarning("Carrier {UserId} attempted to open unassigned Shipment {ReferecneId}", request.CarrierId, request.ShipmentId);
                var notFoundResult = Result<RetrieveShipmentDto>.NotFound(Errors.ShipmentNotFound("Shipment not found for this carrier."));
                return notFoundResult;
            }
            var result = Result<RetrieveShipmentDto>.Success(shipment);
            return result;
        }
    }
}
