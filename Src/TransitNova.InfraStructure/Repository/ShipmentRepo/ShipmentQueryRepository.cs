using System.Linq.Expressions;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.ShipmentRepo
{
    internal class ShipmentQueryRepository(AppDbContext context, IMapper mapper) : IShipmentQueryRepository
    {
        public async Task<PagedResult<RetrieveShipmentDto>> FilterAsync(ShipmentFilterDto filter, CancellationToken ct)
        {
            IQueryable<Shipment> query = context.Shipments
                .AsNoTracking();

            if (filter.Status != null && filter.Status.Length > 0)
            {
                query = query.Where(sh => filter.Status.Contains(sh.CurrentStatus));
            }

            if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
            {
                var pattern = $"%{filter.SearchTerm.Trim()}%";
                query = query.Where(sh =>
                    EF.Functions.Like(sh.TrackingNumber, pattern) ||
                    EF.Functions.Like(sh.PickupAddress, pattern) ||
                    EF.Functions.Like(sh.DeliveryAddress, pattern) ||
                    EF.Functions.Like(sh.Sender.FirstName + " " + sh.Sender.LastName, pattern) ||
                    EF.Functions.Like(sh.Receiver.FirstName + " " + sh.Receiver.LastName, pattern) ||
                    EF.Functions.Like(sh.Sender.City.Name, pattern) ||
                    EF.Functions.Like(sh.Receiver.City.Name, pattern));
            }

            if (filter.From.HasValue)
            {
                query = query.Where(sh => sh.PickupDate >= filter.From.Value);
            }

            if (filter.To.HasValue)
            {
                query = query.Where(sh => sh.ActualDeliveryDate <= filter.To.Value);
            }

            if (filter.WarehouseId.HasValue)
            {
                query = query.Where(sh => sh.Trip != null ? sh.Trip.WarehouseId == filter.WarehouseId.Value : false);
            }

            if (filter.SenderId.HasValue)
            {
                query = query.Where(sh => sh.SenderId == filter.SenderId.Value);
            }

            if (filter.Mode.HasValue)
            {
                query = query.Where(sh => sh.Mode == filter.Mode.Value);
            }

            var totalCount = await query.CountAsync(ct);

            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            query = query
                .OrderByDescending(sh => sh.CreatedAt)
                .ThenBy(sh => sh.Id)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            var filteredShipments = await query
                .ProjectTo<RetrieveShipmentDto>(mapper.ConfigurationProvider)
                .ToListAsync(ct);

            return PagedResult<RetrieveShipmentDto>.From(filteredShipments, totalCount, pageNumber, pageSize);
        }

        public async Task<RetrieveShipmentSummaryDto?> GetByTrackingNumberAsync(string trackingNumber, CancellationToken ct)
            => await context.Shipments
                .AsNoTracking()
                .Where(sh => sh.TrackingNumber == trackingNumber)
                .Select(sh => new RetrieveShipmentSummaryDto
                {
                    Id = sh.Id,
                    TrackingNumber = sh.TrackingNumber,
                    CurrentStatus = sh.CurrentStatus,
                    ShipmentType = sh.ShipmentType,
                    CreatedAt = sh.CreatedAt,
                    SenderCity = sh.Sender.City.Name,
                    ReceiverCity = sh.Receiver.City.Name,
                    ReceiverName = sh.Receiver.FullName,
                    SenderName = sh.Sender.FullName,
                    Weight = sh.PackageSpecification.Weight
                })
                .FirstOrDefaultAsync(ct);

        public Task<RetrieveShipmentDto?> CreateShipmentForUserAsync(Guid shipmentId, CancellationToken ct)
            => GetShipmentAsync(sh => sh.Id == shipmentId, ct);

        public async Task<RetrieveShipmentDto?> GetShipmentAsync(Expression<Func<Shipment, bool>> predicate, CancellationToken ct)
            => await context.Shipments
                .AsNoTracking()
                .Where(predicate)
                .ProjectTo<RetrieveShipmentDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ct);

        public async Task<IEnumerable<Shipment>> GetShipmentsAssignedToCarrierAsync(ShipmentStatuses shipmentStatus, Guid carrierId, CancellationToken ct)
            => await context.Shipments
                .Where(sh => sh.CurrentStatus == shipmentStatus &&
                             sh.ShipmentStates.Any(ss => ss.CarrierId == carrierId))
                .Include(sh => sh.Receiver)
                .Include(sh => sh.Sender)
                .Include(sh => sh.ShipmentStates)
                .ToListAsync(ct);

        public async Task<IEnumerable<RetrieveShipmentStatusDto>> GetShipmentHistoriesAsync(Guid shipmentId, CancellationToken cancellationToken)
             => await context.ShipmentStatuses
                       .AsNoTracking()
                       .Where(ss => ss.Shipment.Id == shipmentId)
                       .OrderBy(ss => ss.CreatedAt)
                       .ProjectTo<RetrieveShipmentStatusDto>(mapper.ConfigurationProvider)
                       .ToListAsync(cancellationToken);

        public async Task<Dictionary<ShipmentStatuses, int>> GetShipmentCountInStatusAsync(CancellationToken cancellationToken)
            => await context.Shipments
                    .AsNoTracking()
                    .GroupBy(st => st.CurrentStatus)
                    .Select(g => new
                    {
                        Status = g.Key,
                        Count = g.Count()
                    })
                    .ToDictionaryAsync(g => g.Status, g => g.Count, cancellationToken);

        public async Task<Shipment?> GetShipmentInStatusAsync(Guid shipmentId, ShipmentStatuses shipmentStatus, CancellationToken ct, bool includes = false)
        {
            var query = context.Shipments
              .Where(sh => sh.Id == shipmentId && sh.CurrentStatus == shipmentStatus);
            if (includes)
            {
                query = query
                    .Include(sh => sh.Receiver)
                    .Include(sh => sh.ShipmentStates)
                    .Include(sh => sh.Sender);
                return await query.FirstOrDefaultAsync(ct);
            }
            return await query.FirstOrDefaultAsync(ct);
        }

        public async Task<Shipment?> GetEntityAsync(Guid shipmentId, CancellationToken ct)
            => await context.Shipments
                .AsQueryable()
                .Where(sh => sh.Id == shipmentId)
                .Include(sh => sh.ShipmentStates)
                .Include(sh => sh.Trip!)
                    .ThenInclude(t => t.Shipments)
                .FirstOrDefaultAsync(ct);

        public async Task<Shipment?> GetShipmentForCommandsAsync(Guid shipmentId, CancellationToken ct)
            => await context.Shipments.Where(sh => sh.Id == shipmentId).Include(sh => sh.ShipmentStates).FirstOrDefaultAsync(ct);

        public async Task<string?> GetShipmentTrackingNumberAsync(Guid shipmentId, CancellationToken ct)
        => await context.Shipments
                .AsNoTracking()
                .Where(sh => sh.Id == shipmentId)
                .Select(sh => sh.TrackingNumber)
                .FirstOrDefaultAsync(ct);
    }
}