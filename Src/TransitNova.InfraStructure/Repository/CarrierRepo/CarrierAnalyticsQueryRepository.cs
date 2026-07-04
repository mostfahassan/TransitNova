
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.Domain.Contracts.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.CarrierRepo
{
    internal class CarrierAnalyticsQueryRepository(AppDbContext context) : ICarrierAnalyticsQueryRepository
    {
        public async Task<decimal?> GetAverageRatingAsync(Guid carrierId, CancellationToken ct = default)
            => await context.Carriers
                .Where(c => c.Id == carrierId)
                .AverageAsync(c => (decimal?)c.AverageRating, ct);

        public async Task<decimal?> GetCarrierRevenueAsync(Guid carrierId, CancellationToken ct = default)
        {
           var totalShipmentCost =  await context.Trips.AsNoTracking()
            .Where(t => t.CarrierId == carrierId && t.Status == TripStatus.Completed)
            .SelectMany(t => t.Shipments)
            .Where(s => s.CurrentStatus == ShipmentStatuses.Delivered || s.CurrentStatus == ShipmentStatuses.InWarehouse)
            .SumAsync(s => (decimal?)s.ShipmentCost, ct) ?? 0m;

            return totalShipmentCost * CarrierCommission.CommissionRate;
        }
    }
}
