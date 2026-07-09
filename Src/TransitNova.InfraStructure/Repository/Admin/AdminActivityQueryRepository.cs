
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.Admin
{
    internal class AdminActivityQueryRepository(IDbContextFactory<AppDbContext> contextFactory, IMapper mapper) : IAdminActivityQueryRepository
    {
        public async Task<List<AdminActivityDto>> GetRecentActivitiesAsync(CancellationToken cancellationToken, int count = 10)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            return await context.SystemActivityLogs.AsNoTracking()
                .OrderByDescending(s => s.OccurredAt)
                 .Select(s => new AdminActivityDto
                 {
                     Title = $"{s.Action} {s.EntityType}",
                     Description = s.Description,
                     PerformedBy = s.PerformedByName,
                     OccurredAt = s.OccurredAt
                 })
               .Take(count)
               .ToListAsync(cancellationToken);
        }
        public async Task<List<RetrieveShipmentDto>> GetRecentShipmentsAsync(CancellationToken cancellationToken, int count = 10)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            return await context.Shipments.AsNoTracking()
                .OrderByDescending(s => s.CreatedAt)
                .ProjectTo<RetrieveShipmentDto>(mapper.ConfigurationProvider)
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<Dictionary<ShipmentStatuses, int>> GetShipmentCountInStatusAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

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

        public async Task<List<TopCarrierDto>> GetTopCarriersAsync(CancellationToken cancellationToken, int count = 10)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Carriers.AsNoTracking()
                .OrderByDescending(c => c.SuccessRate)
                .Select(c => new TopCarrierDto
                {
                    CarrierId = c.Id,
                    FullName = c.FullName,
                    DeliveredShipments = c.CompletedShipmentsCount,
                    Rating = c.AverageRating
                }).Take(count)
                .ToListAsync(cancellationToken);

        }

        public async Task<List<TopOperationManagerDto>> GetTopOperationManagersAsync(CancellationToken cancellationToken, int count = 10)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            return await context.OperationManagerProfiles.AsNoTracking()
                .OrderByDescending(c => c.HandledShipments.Count)
                .Select(op => new TopOperationManagerDto
                {
                    OperationManagerId = op.Id,
                    ApprovedShipments = op.HandledShipments.Count,
                    FullName = op.FullName
                })
                .Take(count)
                .ToListAsync(cancellationToken);
        }

        public async Task<RevenueSummaryDto> GetRevenueSummaryAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);

            var now = DateTime.UtcNow;
            var today = now.Date;
            var weekStart = today.AddDays(-(int)today.DayOfWeek);
            var monthStart = new DateTime(now.Year, now.Month, 1);

            var revenue = await context.Shipments
                .AsNoTracking()
                .Where(s =>
                    s.CurrentStatus == ShipmentStatuses.Delivered ||
                    s.CurrentStatus == ShipmentStatuses.InWarehouse)
                .GroupBy(_ => 1)
                .Select(g => new RevenueSummaryDto
                {
                    TotalRevenue = g.Sum(x => x.ShipmentCost),

                    MonthlyRevenue = g
                        .Where(x => x.CreatedAt >= monthStart)
                        .Sum(x => x.ShipmentCost),

                    WeeklyRevenue = g
                        .Where(x => x.CreatedAt >= weekStart)
                        .Sum(x => x.ShipmentCost),

                    DailyRevenue = g
                        .Where(x => x.CreatedAt >= today)
                        .Sum(x => x.ShipmentCost)
                })
                .FirstOrDefaultAsync(cancellationToken);

            return revenue ?? new RevenueSummaryDto();
        }
    }
}
