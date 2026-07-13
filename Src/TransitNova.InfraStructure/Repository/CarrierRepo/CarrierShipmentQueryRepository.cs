using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.Interfaces.Repositories.CarrierRepository;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.CarrierRepo;

public class CarrierShipmentQueryRepository(
    AppDbContext context,
    IMapper mapper,
    ILogger<CarrierShipmentQueryRepository> logger)
    : ICarrierShipmentQueryRepository
{
    public async Task<RetrieveShipmentDto?> GetCarrierShipmentAsync(Guid carrierId, Guid shipmentId, CancellationToken ct = default)
    {
        logger.LogDebug("Fetching shipments for Carrier {UserId} and Shipment {ReferecneId}", carrierId, shipmentId);

        var shipment = await context.Shipments
            .AsNoTracking()
            .Where(sh => sh.ShipmentStates.Any(ss =>
                ss.CarrierId == carrierId &&
                ss.ShipmentId == shipmentId))
            .ProjectTo<RetrieveShipmentDto>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync(ct);

        return shipment;
    }

    public async Task<PagedResult<RetrieveShipmentDto>> GetCarrierShipmentsAsync(Guid carrierId, CarrierShipmentFilterDto filter, CancellationToken ct = default)
    {

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
        var pageSize = filter.PageSize <= 0 ? 12 : filter.PageSize;

        var shipments = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ProjectTo<RetrieveShipmentDto>(mapper.ConfigurationProvider)
            .ToListAsync(ct);

        return PagedResult<RetrieveShipmentDto>.From(shipments,totalCount,pageNumber,pageSize);
    }

    public async Task<Dictionary<ShipmentStatuses, int>> GetCarrierShipmentCountInStatusAsync(Guid carrierId, CancellationToken ct = default)
    {
        return await context.Shipments
            .AsNoTracking()
            .Where(sh => !sh.IsDeleted && sh.ShipmentStates.Any(ss => ss.CarrierId == carrierId))
            .GroupBy(sh => sh.CurrentStatus)
            .ToDictionaryAsync(group => group.Key, group => group.Count(), ct);
    }
}