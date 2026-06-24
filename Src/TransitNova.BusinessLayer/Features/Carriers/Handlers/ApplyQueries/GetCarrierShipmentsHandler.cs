using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.Domain.Entities.MainEntities;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries
{
    public sealed class GetCarrierShipmentsHandler(
        ICarrierShipmentQueryRepository carrierShipmentRepository,
        ILogger<GetCarrierShipmentsHandler> logger)
        : IQueryHandler<GetCarrierShipmentsQuery, Result<CarrierShipmentListDto>>
    {
        public async Task<Result<CarrierShipmentListDto>> Handle(GetCarrierShipmentsQuery request, CancellationToken ct)
        {
            logger.LogInformation("Handling GetCarrierShipmentsQuery for CarrierId: {CarrierId}", request.CarrierId);

            //===== Normalize Pagination Value ============
            var pageNumber = request.Filter.PageNumber <= 0 ? 1 : request.Filter.PageNumber;
            var pageSize = request.Filter.PageSize <= 0 ? 12 : request.Filter.PageSize;


            request.Filter.PageNumber = pageNumber;
            request.Filter.PageSize = pageSize;

            logger.LogInformation("Fetching shipments for UserId: {UserId} with PageNumber: {PageNumber} and PageSize: {PageSize}", request.CarrierId, pageNumber, pageSize);
            var shipments = await carrierShipmentRepository.GetCarrierShipmentsAsync(request.CarrierId, request.Filter, ct);
            var stats = await carrierShipmentRepository.GetCarrierShipmentCountInStatusAsync(request.CarrierId, ct);

            logger.LogInformation("Fetched {ShipmentCount} shipments and {StatusCount} status statistics for UserId: {UserId}", shipments.TotalCount, stats.Count, request.CarrierId);
            var model = new CarrierShipmentListDto
            {
                Shipments = shipments,
                Statistics = stats.Select(s => new CarrierStatusStatDto { Status = s.Key, Count = s.Value }).ToList()
            };
            var result = Result<CarrierShipmentListDto>.Success(model);
            return result;
        }
    }
}
