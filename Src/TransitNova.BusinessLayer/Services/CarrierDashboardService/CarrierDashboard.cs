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
            var carrierSummary = await carrierRepository.GetCarrierProfileSummaryAsync(carrierId, cancellationToken);
            if (carrierSummary == null || string.IsNullOrWhiteSpace(carrierSummary.FullName))
            {
                logger.LogWarning("Carrier dashboard rejected because AppUser {CarrierId} has no Carrier profile", carrierId);
                var notFoundResult = Result<CarrierDashboardDto>.NotFound(Errors.CarrierNotFound("Carrier profile not found."));
                return notFoundResult;
            }
            var filter = new CarrierShipmentFilterDto { PageNumber = 1, PageSize = 15 };
           
            var recentShipmentsTask = carrierDashboardRepository.GetCarrierShipmentsAsync(carrierSummary.Id, filter, cancellationToken);
            
            var statsTask = carrierDashboardRepository.GetCarrierShipmentCountInStatusAsync(carrierSummary.Id, cancellationToken);

            var tripsTask =  carrierDashboardRepository.GetCarrierTripsAsync(carrierSummary.Id, cancellationToken);

            var revenueTask = carrierDashboardRepository.GetCarrierRevenueAsync(carrierSummary.Id, cancellationToken);

            await Task.WhenAll(statsTask, tripsTask, revenueTask, recentShipmentsTask);

            var dashboard = CarrierDashboardBuilder.Build(statsTask.Result, tripsTask.Result, revenueTask.Result, carrierSummary.Id,carrierSummary.FullName, recentShipmentsTask.Result.Data);
            return Result<CarrierDashboardDto>.Success(dashboard);
        }
    }
}
