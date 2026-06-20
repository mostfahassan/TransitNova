using Microsoft.EntityFrameworkCore;

using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Trip;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.TripRepository
{
    public class TripQueryRepository( AppDbContext context) : ITripQueryRepository
    {
        public async Task<List<CarrierTripDto>> GetCarrierTripsAsync(Guid carrierId, CancellationToken cancellationToken)
           => await context.Trips
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
        

        public async Task<CarrierTripDto?> GetCarrierTripAsync(Guid carrierId, Guid tripId, CancellationToken cancellationToken)
             => await ProjectCarrierTripDetails(context.Trips.AsNoTracking()
                .Where(t => t.CarrierId == carrierId && t.Id == tripId))
                .FirstOrDefaultAsync(cancellationToken);
        

        public async Task<Trip?> GetTripByIdAsync(Guid tripId, CancellationToken cancellationToken)
            => await context.Trips
                .Where(t => t.Id == tripId)
                .Include(t => t.Shipments)
                    .ThenInclude(s => s.ShipmentStates)
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<TripDetailsDto?> GetTripAsync(Guid tripId, CancellationToken cancellationToken)
            => await ProjectTripDetails(
                context.Trips.AsNoTracking()
                .Where(t => t.Id == tripId))
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<IEnumerable<TripDetailsDto>> GetTripsAsync(CancellationToken cancellationToken)
            => await ProjectTripDetails(
                context.Trips.AsNoTracking())
                .OrderByDescending(t => t.PlannedDate)
                .ToListAsync(cancellationToken);

        public async Task<IEnumerable<TripDetailsDto>> GetTripByTypeAsync(TripType tripType, CancellationToken cancellationToken)
            => await ProjectTripDetails(
                context.Trips.AsNoTracking()
                .Where(t => t.TripType == tripType))
                .OrderByDescending(t => t.PlannedDate)
                .ToListAsync(cancellationToken);

        public async Task<IEnumerable<TripDetailsDto>> GetTripByStatusAsync(TripStatus tripStatus, CancellationToken cancellationToken)
            => await ProjectTripDetails(
                context.Trips.AsNoTracking()
                .Where(t => t.Status == tripStatus))
                .OrderByDescending(t => t.PlannedDate)
                .ToListAsync(cancellationToken);

        public async Task<TripDetailsDto?> GetTripByTypeAsync(Guid tripId, TripType tripType, CancellationToken cancellationToken)
            => await ProjectTripDetails(
                 context.Trips.AsNoTracking()
                .Where(t => t.Id == tripId && t.TripType == tripType))
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<TripDetailsDto?> GetTripByStatusAsync(Guid tripId, TripStatus tripStatus, CancellationToken cancellationToken)
            => await ProjectTripDetails(context.Trips.AsNoTracking().Where(t => t.Id == tripId && t.Status == tripStatus))
                .FirstOrDefaultAsync(cancellationToken);














        static IQueryable<TripDetailsDto> ProjectTripDetails(IQueryable<Trip> query)
            => query.Select(t => new TripDetailsDto
            {
                Id = t.Id,
                CarrierId = t.CarrierId,
                CarrierName = t.Carrier.FullName,
                CarrierPhoneNumber = t.Carrier.PhoneNumber,
                WarehouseId = t.WarehouseId,
                WarehouseName = t.Warehouse.Name,
                WarehouseAddress = t.Warehouse.Address,
                TripType = t.TripType,
                Status = t.Status,
                PlannedDate = t.PlannedDate,
                StartTime = t.StartTime,
                EndTime = t.EndTime,
                TotalShipments = t.TotalShipments,
                Shipments = t.Shipments.Select(sh => new RetrieveShipmentDto
                {
                    Id = sh.Id,
                    ReceiverId = sh.ReceiverId,
                    SenderId = sh.SenderId,
                    TrackingNumber = sh.TrackingNumber,
                    Receiver = new UserSummaryDto
                    {
                        FullName = sh.Receiver.FullName,
                        Email = sh.Receiver.Email,
                        PhoneNumber = sh.Receiver.PhoneNumber,
                        Address = sh.Receiver.Address
                    },
                    Sender = new UserSummaryDto
                    {
                        FullName = sh.Sender.FullName,
                        Email = sh.Sender.Email,
                        PhoneNumber = sh.Sender.PhoneNumber,
                        Address = sh.Sender.Address
                    },
                    DeliveryAddress = sh.DeliveryAddress,
                    PickupAddress = sh.PickupAddress,
                    PackageSpecification = new PackageSpecificationDto
                    {
                        Weight = sh.PackageSpecification.Weight,
                        Width = sh.PackageSpecification.Width,
                        Height = sh.PackageSpecification.Height,
                        Length = sh.PackageSpecification.Length
                    },
                    Currency = sh.Currency,
                    TransportationMode = sh.Mode,
                    CurrentStatus = sh.CurrentStatus,
                    ShippingCost = sh.ShipmentCost,
                    EstimatedDeliveryDate = sh.EstimatedDeliveryDate,
                    ShipmentType = sh.ShipmentType,
                    PackageBundleId = sh.PackageBundleId,
                    CreatedAt = sh.CreatedAt
                }).ToList()
            });


        static IQueryable<CarrierTripDto> ProjectCarrierTripDetails(IQueryable<Trip> query)

           => query.Select(t => new CarrierTripDto
           {
               Id = t.Id,
               TripType = t.TripType,
               Status = t.Status,
               PlannedDate = t.PlannedDate,
               StartTime = t.StartTime,
               EndTime = t.EndTime,
               WarehouseName = t.Warehouse.Name,
               WarehouseAddress = t.Warehouse.Address,
               ShipmentCount = t.Shipments.Count,
               Shipments = t.Shipments.Select(sh => new RetrieveShipmentDto
               {
                   Id = sh.Id,
                   ReceiverId = sh.ReceiverId,
                   SenderId = sh.SenderId,
                   TrackingNumber = sh.TrackingNumber,
                   Receiver = new UserSummaryDto
                   {
                       FullName = sh.Receiver.FullName,
                       Email = sh.Receiver.Email,
                       PhoneNumber = sh.Receiver.PhoneNumber,
                       Address = sh.Receiver.Address
                   },
                   Sender = new UserSummaryDto
                   {
                       FullName = sh.Sender.FullName,
                       Email = sh.Sender.Email,
                       PhoneNumber = sh.Sender.PhoneNumber,
                       Address = sh.Sender.Address
                   },
                   DeliveryAddress = sh.DeliveryAddress,
                   PickupAddress = sh.PickupAddress,
                   PackageSpecification = new PackageSpecificationDto
                   {
                       Weight = sh.PackageSpecification.Weight,
                       Width = sh.PackageSpecification.Width,
                       Height = sh.PackageSpecification.Height,
                       Length = sh.PackageSpecification.Length
                   },
                   Currency = sh.Currency,
                   TransportationMode = sh.Mode,
                   CurrentStatus = sh.CurrentStatus,
                   ShippingCost = sh.ShipmentCost,
                   EstimatedDeliveryDate = sh.EstimatedDeliveryDate,
                   ShipmentType = sh.ShipmentType,
                   PackageBundleId = sh.PackageBundleId,
                   CreatedAt = sh.CreatedAt
               }).ToList()
           });



    }
}
