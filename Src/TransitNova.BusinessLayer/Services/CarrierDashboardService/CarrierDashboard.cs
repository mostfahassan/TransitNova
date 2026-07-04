using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CarrierDashboard;
namespace TransitNova.BusinessLayer.Services.CarrierDashboardService
{
    internal class CarrierDashboard(ICarrierDashboardRepository carrierDashboardRepository,
        ICarrierQueryRepository carrierRepository,
        ILogger<CarrierDashboard> logger) : ICarrierDashboard
    {
        public async Task<Result<CarrierDashboardDto>> BuildAsync(Guid carrierId, CancellationToken cancellationToken)
        {
            var carrier = await carrierRepository.GetCarrierAsync(c => c.Id == carrierId || c.AppUserId == carrierId, cancellationToken);
            if (carrier is null)
            {
                logger.LogWarning("Carrier dashboard rejected because AppUser {CarrierId} has no Carrier profile", carrierId);
                var notFoundResult = Result<CarrierDashboardDto>.NotFound(Errors.CarrierNotFound("Carrier profile not found."));
                return notFoundResult;
            }
            var filter = new CarrierShipmentFilterDto { PageNumber = 1, PageSize = 10 };
           
            var recentShipmentsTask = carrierDashboardRepository.GetCarrierShipmentsAsync(carrier.Id, filter, cancellationToken);
            
            var statsTask = carrierDashboardRepository.GetCarrierShipmentCountInStatusAsync(carrier.Id, cancellationToken);

            var tripsTask =  carrierDashboardRepository.GetCarrierTripsAsync(carrier.Id, cancellationToken);

            var revenueTask = carrierDashboardRepository.GetCarrierRevenueAsync(carrier.Id, cancellationToken);

            await Task.WhenAll(statsTask, tripsTask, revenueTask, recentShipmentsTask);

            var dashboard = CarrierDashboardBuilder.Build(statsTask.Result, tripsTask.Result, revenueTask.Result, carrier, recentShipmentsTask.Result.Data);
            return Result<CarrierDashboardDto>.Success(dashboard);
        }
    }
}
