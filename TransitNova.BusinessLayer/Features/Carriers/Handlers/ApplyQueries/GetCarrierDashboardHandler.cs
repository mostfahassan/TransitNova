using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries
{
    public sealed class GetCarrierDashboardHandler(
        ICarrierQueryRepository carrierRepository,
        ICarrierShipmentQueryRepository carrierShipmentrepository,
        ICarrierAnalyticsQueryRepository carrierAnalytics,
        ITripQueryRepository tripRepository,
        ILogger<GetCarrierDashboardHandler> logger)
        :IQueryHandler<GetCarrierDashboardQuery, Result<CarrierDashboardDto>>
    {
        public async Task<Result<CarrierDashboardDto>> Handle(GetCarrierDashboardQuery request, CancellationToken ct)
        {
            var carrier = await carrierRepository.GetCarrierAsync(c => c.Id == request.carrierId, ct);
            if (carrier is null)
            {
                logger.LogWarning("Carrier dashboard rejected because AppUser {CarrierId} has no Carrier profile", request.carrierId);
                var notFoundResult = Result<CarrierDashboardDto>.NotFound(Errors.CarrierNotFound("Carrier profile not found."));
                return notFoundResult;
            }
            var filter = new CarrierShipmentFilterDto { PageNumber = 1, PageSize = 10 };
            var recentShipments= await carrierShipmentrepository.GetCarrierShipmentsAsync(carrier.Id, filter, ct);

            var stats = await carrierShipmentrepository.GetCarrierShipmentCountInStatus(carrier.Id, ct);

            var trips = await  tripRepository.GetCarrierTripsAsync(carrier.Id, ct);

            var revenue = await carrierAnalytics.GetCarrierRevenueAsync(carrier.Id, ct) ?? default;

            var dashboard = CarrierDashboardBuilder.Build(stats, trips, revenue, carrier, recentShipments.Data);

            logger.LogInformation("Carrier dashboard loaded successfully for Carrier {UserId}", carrier.Id);

            var result = Result<CarrierDashboardDto>.Success(dashboard);
            return result;
        }
    }
}
