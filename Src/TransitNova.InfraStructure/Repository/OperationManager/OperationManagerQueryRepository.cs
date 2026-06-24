
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.UserProfile.OperationManager;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.OperationManager
{
    internal class OperationManagerQueryRepository(AppDbContext context) : IOperationManagerQueryRepository
    {
        public async Task<List<OperationManagerProfileDto>> GetActiveAsync(CancellationToken cancellationToken)
            => await ProjectToProfileDto(context.OperationManagerProfiles.AsNoTracking().Where(op => op.CurrentState))
                            .ToListAsync(cancellationToken);


        public async Task<List<OperationManagerProfileDto>> GetAllAsync(CancellationToken cancellationToken)
                     => await ProjectToProfileDto(context.OperationManagerProfiles.AsNoTracking())
                             .ToListAsync(cancellationToken);

        public async Task<OperationManagerProfileDto?> GetOperationManagerProfileAsync(Guid id, CancellationToken cancellationToken)
            => await ProjectToProfileDto(context.OperationManagerProfiles.AsNoTracking().Where(op => op.Id == id))
                           .FirstOrDefaultAsync(cancellationToken);


        public async Task<Guid> GetUserIdAsync(Guid userId, CancellationToken cancellationToken)
            => await context.OperationManagerProfiles.Where(op => op.AppUserId == userId).Select(op => op.Id).FirstOrDefaultAsync(cancellationToken);



        public async Task<PagedResult<RetrieveShipmentSummaryDto>> GetHandledShipmentsByOperationManagerAsync(Guid operationManagerId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {
            var query = context.Shipments
                        .AsNoTracking()
                        .Where(sh => sh.HandledById == operationManagerId &&
                                     sh.UpdatedBy == operationManagerId.ToString());

            var totalCount = await query.CountAsync(cancellationToken);

            var shipments = await query
                         .OrderByDescending(sh => sh.UpdatedAt)
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize)
                         .Select(sh => new RetrieveShipmentSummaryDto
                         {
                             Id = sh.Id,
                             SenderCity = sh.Sender.City.Name,
                             ReceiverCity = sh.Receiver.City.Name,
                             ShipmentType = sh.ShipmentType,
                             CurrentStatus = sh.CurrentStatus,
                             ShippinCost = sh.ShipmentCost,
                             TrackingNumber = sh.TrackingNumber,
                             EstimatedDeliveryDate =
                                 sh.CurrentStatus != ShipmentStatuses.Delivered
                                     ? sh.EstimatedDeliveryDate
                                     : sh.ActualDeliveryDate
                         })
                         .ToListAsync(cancellationToken);

            return PagedResult<RetrieveShipmentSummaryDto>.From(shipments, totalCount, pageNumber, pageSize);
        }

        public async Task<PagedResult<CarrierSummaryDetailsDto>> GetHandledCarriersByOperationManagerAsync(Guid operationManagerId, int pageNumber, int pageSize, CancellationToken cancellationToken)
        {

            var query = context.Carriers.AsNoTracking()
                .Where(sh => sh.HandlerId == operationManagerId &&
                                     sh.UpdatedBy == operationManagerId.ToString());

            var totalCount = await query.CountAsync(cancellationToken);

            var carriers = await query.OrderByDescending(sh => sh.UpdatedAt)
                         .Skip((pageNumber - 1) * pageSize)
                         .Take(pageSize)
                         .Select(c => new CarrierSummaryDetailsDto
                         {
                             Id = c.Id,
                             FullName = c.FullName,
                             PhoneNumber = c.PhoneNumber,
                             Code = c.Code,
                             Status = c.Status,
                             AssignedShipmentsCount = c.AssignedShipmentsCount,
                             ActiveTripsCount = c.Trips.Count(t => t.Status == TripStatus.Active),
                             Rating = c.AverageRating
                         }).ToListAsync(cancellationToken);


            return PagedResult<CarrierSummaryDetailsDto>.From(carriers, totalCount, pageNumber, pageSize);
        }

        static IQueryable<OperationManagerProfileDto> ProjectToProfileDto(IQueryable<OperationManagerProfile> query)
           => query.Select(op => new OperationManagerProfileDto
           {
               Id = op.Id,
               Address = op.Address,
               PhoneNumber = op.PhoneNumber,
               Email = op.Email,
               CityName = op.City.Name,
               GovernmentName = op.City.Government.Name,
               CountryName = op.City.Government.Country.Name,
               UserType = op.UserType,
               TotalCarriertHandled = op.HandledCarriers.Count(),
               TotalShipmentHandled = op.HandledShipments.Count(),
           });

        public async Task<string?> GetOperationManagerNameAsync(Guid userId, CancellationToken cancellationToken)
         => await context.OperationManagerProfiles.AsNoTracking()
            .Where(op => op.AppUserId == userId)
            .Select(op => op.FullName)
            .FirstOrDefaultAsync(cancellationToken);

        public async Task<List<Guid>> GetOperationManagersIdsAsync(CancellationToken cancellationToken)
          => await context.OperationManagerProfiles
                .AsNoTracking()
                .Select(op => op.Id)
                .ToListAsync(cancellationToken);
        
    }


}
