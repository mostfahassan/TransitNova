
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.Admin
{
    internal class AdminDashboard(IDbContextFactory<AppDbContext> contextFactory , IMapper mapper) : IAdminDashboardBuilder
    {
        public async Task<OperationManagerStats> OperationManagerStatsAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            var operationManagerHealthSummary =   await context.OperationManagerProfiles
                .AsNoTracking()
                .GroupBy(_ => 1)
                .Select(g => new OperationManagerStats
                {
                    TotalOperationManagers = g.Count(),
                    ActiveOperationManagers = g.Count(op => op.CurrentState)

                }).FirstOrDefaultAsync(cancellationToken);

            return operationManagerHealthSummary ?? new OperationManagerStats();
        }

        public async Task<ShipmentStats> ShipmentsStatsAsync(CancellationToken cancellationToken)
        {
            ShipmentStatuses[] ActiveStatuses = [ShipmentStatuses.OutForPickup, ShipmentStatuses.OutForDelivery, ShipmentStatuses.InTransit];
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            var shipmentHealthSummary = await context.Shipments
                        .AsNoTracking()
                        .GroupBy(_ => 1)
                        .Select(g => new ShipmentStats
                        {
                            TotalShipments = g.Count(),
                            CancelledShipmentRate = g.Count(sh => sh.CurrentStatus == ShipmentStatuses.Cancelled),
                            ActiveShipments = g.Count(sh => ActiveStatuses.Contains(sh.CurrentStatus)),
                            PendingShipments = g.Count(sh => sh.CurrentStatus == ShipmentStatuses.Pending),
                            DeliveredShipments = g.Count(sh => sh.CurrentStatus == ShipmentStatuses.Delivered),
                        })
                        .FirstOrDefaultAsync(cancellationToken);

            return shipmentHealthSummary is null ? new ShipmentStats() : shipmentHealthSummary;
        }
        public async Task<CarrierStats> GetCarriersActivityStatsAsync(CancellationToken cancellationToken)
        {
            CarrierStatus[] busyStatus = [CarrierStatus.AssignedToDeliveryShipment, CarrierStatus.OnTrip, CarrierStatus.AssignedToPickUpShipment];
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            var carrierHealthSummary = await context.Carriers.AsNoTracking()
                .GroupBy(_ =>1)
                .Select (g => new CarrierStats
                {
                    TotalCarriers = g.Count(),
                    ActiveCarriers = g.Count(c => c.CurrentState),
                    AvailableCarriers = g.Count(c => c.Status == CarrierStatus.Available),
                    BusyCarriers = g.Count(c => busyStatus.Contains(c.Status)),
                    DeliverySuccessRate = g.Average(c => c.SuccessRate),
                    AverageCarrierRating = g.Average(c => c.AverageRating)

                }).FirstOrDefaultAsync(cancellationToken);

            return carrierHealthSummary is null ? new CarrierStats () : carrierHealthSummary;
        }

        public async Task<UserStatistics> UsersStatsAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.UserProfiles.AsNoTracking()
                .GroupBy(_ => 1)
                .Select(g => new UserStatistics
                {
                    TotalUsers = g.Count(),
                    ActiveUsers = g.Count(u => u.CurrentState)

                }).FirstOrDefaultAsync(cancellationToken) ?? new UserStatistics();
        }

        public async Task<TripsStatistics> TripsStatsAsync(CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Trips.AsNoTracking()
                .GroupBy(_ => 1)
                .Select(g => new TripsStatistics
                {
                    TotalTrips = g.Count(),
                    ActiveTrips = g.Count(t => t.Status == TripStatus.Active),
                    PlannedTrips = g.Count(t => t.Status == TripStatus.Planned),
                    CompletedTrips = g.Count(t => t.Status == TripStatus.Completed),

                }).FirstOrDefaultAsync(cancellationToken) ?? new TripsStatistics();
               
        }

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
