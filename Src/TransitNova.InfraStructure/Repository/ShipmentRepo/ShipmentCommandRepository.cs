

using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.ShipmentRepo
{
    internal class ShipmentCommandRepository(AppDbContext context,ILogger<ShipmentCommandRepository> logger) : IShipmentCommandRepository
    {
        public async Task AddAsync(Shipment shipment, CancellationToken ct)
        {
            logger.LogDebug("Adding new shipment to context. SenderId: {SenderId}", shipment.SenderId);
            await context.Shipments.AddAsync(shipment, ct);
        }
        public async Task UpdateAsync(Shipment shipment, CancellationToken ct)
        {
            logger.LogDebug("Updating shipment {ReferecneId} in context", shipment.Id);
            context.Shipments.Update(shipment); 
        }
    }
}
