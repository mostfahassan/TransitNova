
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.ShipmentRepo
{
    internal class ShipmentRulesRepository(AppDbContext context) : IShipmentRulesRepository
    {
        public Task<bool> IsEditableAsync(Guid shipmentId, ShipmentStatuses[] status, CancellationToken ct) 
                 => context.Shipments.Where(s => s.Id == shipmentId).AnyAsync(s => status.Contains(s.CurrentStatus), ct);

        public async Task<bool> ExistsAsync(Guid shipmentId , CancellationToken ct)
                 => await context.Shipments.AnyAsync(sh=>sh.Id == shipmentId, ct);

        public async Task<bool> CanRateDeliveryCarrierAsync(Guid shipmentId, Guid carrierId, Guid receiverId, CancellationToken ct)
          => await context.Shipments.AnyAsync(sh => sh.Id == shipmentId
             && sh.CurrentStatus == ShipmentStatuses.Delivered
             && sh.Sender.AppUserId == receiverId
             && sh.ShipmentStates.Any(ss => ss.CarrierId == carrierId
             && ss.Carrier != null &&
              (
                ss.Carrier.Status == CarrierStatus.Available ||
                ss.Carrier.Status == CarrierStatus.Unavailable
              )));
        public async Task<bool> CanRatePickupCarrierAsync(Guid shipmentId, Guid carrierId, Guid senderId, CancellationToken ct)
          => await context.Shipments.AnyAsync(sh => sh.Id == shipmentId
             && sh.CurrentStatus == ShipmentStatuses.InWarehouse
             && sh.Sender.AppUserId == senderId
             && sh.ShipmentStates.Any(ss => ss.CarrierId == carrierId
             && ss.Carrier != null &&
              (
                ss.Carrier.Status == CarrierStatus.Available ||
                ss.Carrier.Status == CarrierStatus.Unavailable
              )));

    }
}
