using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.Domain.Contracts.Carrier;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.CarrierRepo
{
    internal class CarrierDashboardRepository(IDbContextFactory<AppDbContext> contextFactory, IMapper mapper) : ICarrierDashboardRepository
    {
        public async Task<decimal> GetCarrierRevenueAsync(Guid carrierId, CancellationToken ct)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            var totalShipmentCost = await context.Trips.AsNoTracking()
           .Where(t => t.CarrierId == carrierId && t.Status == TripStatus.Completed)
           .SelectMany(t => t.Shipments)
           .Where(s => s.CurrentStatus == ShipmentStatuses.Delivered || s.CurrentStatus == ShipmentStatuses.InWarehouse)
           .SumAsync(s => (decimal?)s.ShipmentCost, ct) ?? 0m;
            if (totalShipmentCost <= 0)
             return 0;

            return totalShipmentCost * CarrierCommission.CommissionRate ;
        }

        public async Task<Dictionary<ShipmentStatuses, int>> GetCarrierShipmentCountInStatusAsync(Guid carrierId, CancellationToken ct)
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            
            return await context.Shipments
           .AsNoTracking()
           .Where(sh => !sh.IsDeleted && sh.ShipmentStates.Any(ss => ss.CarrierId == carrierId))
           .GroupBy(sh => sh.CurrentStatus)
           .ToDictionaryAsync(group => group.Key, group => group.Count(), ct);
        }

        public async Task<PagedResult<RetrieveShipmentDto>> GetCarrierShipmentsAsync(Guid carrierId, CarrierShipmentFilterDto filter, CancellationToken ct )
        {
            await using var context = await contextFactory.CreateDbContextAsync(ct);
            var query = context.Shipments
           .AsNoTracking()
           .Where(sh => !sh.IsDeleted && sh.ShipmentStates.Any(ss => ss.CarrierId == carrierId));
            if (filter.Status.HasValue)
            {
                query = query.Where(sh => sh.CurrentStatus == filter.Status.Value);
            }

            if (filter.Mode.HasValue)
            {
                query = query.Where(sh => sh.Mode == filter.Mode.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var term = filter.SearchTerm.Trim().ToLower();
                query = query.Where(sh =>
                    EF.Functions.Like(sh.TrackingNumber.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(sh.Receiver.FirstName.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(sh.Receiver.LastName.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(sh.Sender.FirstName.ToLower(), $"%{term}%") ||
                    EF.Functions.Like(sh.Sender.LastName.ToLower(), $"%{term}%"));
            }

            var totalCount = await query.CountAsync(ct);

            query = (filter.SortBy?.Trim().ToLowerInvariant()) switch
            {
                "trackingnumber" => filter.SortDescending
                    ? query.OrderByDescending(sh => sh.TrackingNumber)
                    : query.OrderBy(sh => sh.TrackingNumber),

                "status" => filter.SortDescending
                    ? query.OrderByDescending(sh => sh.CurrentStatus)
                    : query.OrderBy(sh => sh.CurrentStatus),

                "cost" => filter.SortDescending
                    ? query.OrderByDescending(sh => sh.ShipmentCost)
                    : query.OrderBy(sh => sh.ShipmentCost),

                _ => filter.SortDescending
                    ? query.OrderBy(sh => sh.CreatedAt)
                    : query.OrderByDescending(sh => sh.CreatedAt)
            };

            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 20 : filter.PageSize;

            var shipments = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ProjectTo<RetrieveShipmentDto>(mapper.ConfigurationProvider)
                .ToListAsync(ct);
            return PagedResult<RetrieveShipmentDto>.From(shipments, totalCount, pageNumber, pageSize);
        }

        public async Task<List<CarrierTripDto>> GetCarrierTripsAsync(Guid carrierId, CancellationToken cancellationToken)
        {
            await using var context = await contextFactory.CreateDbContextAsync(cancellationToken);
            return await context.Trips
                .AsNoTracking()
                .Where(t => t.CarrierId == carrierId)
                .OrderByDescending(t => t.PlannedDate)
                .Select(t => new CarrierTripDto
                {
                    Id = t.Id,
                    TripType = t.TripType,
                    Status = t.Status,
                    PlannedDate = t.PlannedDate,
                    StartTime = t.StartTime,
                    EndTime = t.EndTime,
                    WarehouseName = t.Warehouse.Name,
                    WarehouseAddress = t.Warehouse.Address,
                    ShipmentCount = t.Shipments.Count
                })
                .ToListAsync(cancellationToken);
        }
    }
}

