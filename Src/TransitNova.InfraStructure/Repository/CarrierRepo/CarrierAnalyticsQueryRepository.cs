
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.Domain.Enums.Shipment;
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
           => await context.Shipments
           .AsNoTracking()
           .Where(sh => !sh.IsDeleted &&
               sh.CurrentStatus == ShipmentStatuses.Delivered &&
               sh.ShipmentStates.Any(ss => ss.CarrierId == carrierId))
           .SumAsync(sh => (decimal?)sh.ShipmentCost, ct) ?? 0m;
        
    }
}
