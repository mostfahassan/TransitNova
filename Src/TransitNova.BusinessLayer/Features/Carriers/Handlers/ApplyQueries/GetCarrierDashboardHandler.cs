using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Carrier;
using TransitNova.BusinessLayer.Interfaces.Services.CarrierDashboard;
namespace TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries
{
    public sealed class GetCarrierDashboardHandler(ICarrierDashboard carrierDashboard,  ILogger<GetCarrierDashboardHandler> logger)
        :IQueryHandler<GetCarrierDashboardQuery, Result<CarrierDashboardDto>>
    {
        public async Task<Result<CarrierDashboardDto>> Handle(GetCarrierDashboardQuery request, CancellationToken ct)
        {
            logger.LogInformation("Carrier dashboard loaded successfully for Carrier {UserId}", request.carrierId);

            return  await carrierDashboard.BuildAsync(request.carrierId, ct);
        }
    }
}
