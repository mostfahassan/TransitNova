
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;

namespace TransitNova.BusinessLayer.Interfaces.Services.CarrierDashboard
{
    public interface ICarrierDashboard
    {
        Task<Result<CarrierDashboardDto>> BuildAsync(Guid carrierId, CancellationToken cancellationToken);
    }
}
