using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;
using TransitNova.BusinessLayer.Interfaces.Repositories.ShipmentRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.ShipmentRepo
{
    internal class ShipmentQueryRepository(AppDbContext context, IMapper mapper, ILogger<ShipmentQueryRepository> logger) : IShipmentQueryRepository
    {
        public async Task<PagedResult<RetrieveShipmentDto>> FilterAsync(ShipmentFilterDto filter, CancellationToken ct)

        {
            logger.LogDebug("Building filter query with criteria: {@FilterCriteria}", filter);

            IQueryable<Shipment> query = context.Shipments
                .AsNoTracking();

            // Status Filter
            if (filter.Status != null && filter.Status.Length > 0)
            {
                logger.LogTrace("Applying Status filter: {Status}", filter.Status.Length);

                query = query.Where(sh => filter.Status.Contains(sh.CurrentStatus));
            }

            // Pickup Date From
            if (filter.From.HasValue)
            {
                logger.LogTrace("Applying From date filter: {From}", filter.From.Value);

                query = query.Where(sh =>
                    sh.PickupDate >= filter.From.Value);
            }

            // Delivery Date To
            if (filter.To.HasValue)
            {
                logger.LogTrace("Applying To date filter: {To}", filter.To.Value);

                query = query.Where(sh => sh.ActualDeliveryDate <= filter.To.Value);

            }

            // Sender Filter
            if (filter.SenderId.HasValue)
            {
                logger.LogTrace("Applying SenderId filter: {SenderId}", filter.SenderId.Value);

                query = query.Where(sh =>
                    sh.SenderId == filter.SenderId.Value);
            }

            // Transportation Mode Filter
            if (filter.Mode.HasValue)
            {
                logger.LogTrace("Applying TransportationMode filter: {TransportationMode}", filter.Mode.Value);

                query = query.Where(sh =>
                    sh.Mode == filter.Mode.Value);
            }

            // Total Count Before Pagination
            var totalCount = await query.CountAsync(ct);

            // Pagination
            var pageNumber = filter.PageNumber <= 0 ? 1 : filter.PageNumber;
            var pageSize = filter.PageSize <= 0 ? 10 : filter.PageSize;

            logger.LogTrace(
                "Applying pagination: PageNumber={PageNumber}, PageSize={PageSize}",
                pageNumber,
                pageSize);

            query = query
                .OrderByDescending(sh => sh.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize);

            // Projection
            var filteredShipments = await query
                .ProjectTo<RetrieveShipmentDto>(mapper.ConfigurationProvider)
                .ToListAsync(ct);

            logger.LogInformation(
                "Filter query returned {Count} shipments out of {TotalCount}",
                filteredShipments.Count,
                totalCount);

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
                   Weight = sh.PackageSpecification.Weight
                })
                .FirstOrDefaultAsync(ct);

 
        public async Task<RetrieveShipmentDto?> CreateShipmentForUserAsync(Guid shipmentId, CancellationToken ct)
           => await context.Shipments.AsQueryable()
                .AsNoTracking()
                .Where(sh => sh.Id == shipmentId)
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
          
        public async Task<Dictionary<ShipmentStatuses, int>> GetShipmentCountInStatus(CancellationToken cancellationToken)
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
        

        public async Task<Shipment?> GetShipmentForCommands(Guid shipmentId, CancellationToken ct)
            => await context.Shipments.Where(sh => sh.Id == shipmentId).Include(sh => sh.ShipmentStates).FirstOrDefaultAsync(ct);

    }
}
