using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.InfraStructure.Context;
using static System.Runtime.InteropServices.JavaScript.JSType;
namespace TransitNova.InfraStructure.Repository.OperationManager
{
    internal class OperationManagerDashboardRepository(IDbContextFactory<AppDbContext> dbContextFactory) : IOperationManagerDashboardRepository
    {
        public async Task<PagedResult<RetrieveShipmentSummaryDto>> GetShipmentsAsync(CancellationToken ct, int pageNumber = 1, int pageSize = 20)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync(ct);
            var shipments = context.Shipments.AsQueryable()
                .AsNoTracking()
                .Where(sh => sh.CurrentStatus != ShipmentStatuses.Delivered && sh.CurrentStatus != ShipmentStatuses.Cancelled);
               
             var totalCount = await shipments.CountAsync(ct);
            
            var shipmentDtos = await shipments.Select(sh => new RetrieveShipmentSummaryDto
                {
                    Id = sh.Id,
                    TrackingNumber = sh.TrackingNumber,
                    SenderCity = sh.Sender.City.Name,
                    ReceiverCity = sh.Receiver.City.Name,
                    ShippingCost = sh.ShipmentCost,
                    CurrentStatus = sh.CurrentStatus,
                    ShipmentType = sh.ShipmentType,
                    CreatedAt = sh.CreatedAt,
                    Weight = sh.PackageSpecification.Weight,
                    EstimatedDeliveryDate = sh.EstimatedDeliveryDate,
                })
                .OrderByDescending(sh => sh.CreatedAt)
                .ThenBy(sh => sh.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return PagedResult<RetrieveShipmentSummaryDto>.From(shipmentDtos, totalCount, pageNumber, pageSize);

        }

        public async Task<int> TotalHandledCarriersAsync(Guid operationManagerId, CancellationToken cancellationToken)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Carriers
                .AsNoTracking()
                .CountAsync(c => c.HandlerId == operationManagerId, cancellationToken);
        }

        public async Task<int> TotalHandledShipmentsAsync(Guid operationManagerId, CancellationToken cancellationToken)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Shipments
                .AsNoTracking()
                .CountAsync(s => s.HandlerId == operationManagerId, cancellationToken);
        }
        public async Task<Dictionary<ShipmentStatuses, int>> GetShipmentCountInStatusAsync(CancellationToken cancellationToken)
        {
            await using var context = await dbContextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Shipments
                     .AsNoTracking()
                     .GroupBy(st => st.CurrentStatus)
                     .Select(g => new
                     {
                         Status = g.Key,
                         Count = g.Count()
                     })
                    .ToDictionaryAsync(g => g.Status, g => g.Count, cancellationToken);

        }
    }
}
