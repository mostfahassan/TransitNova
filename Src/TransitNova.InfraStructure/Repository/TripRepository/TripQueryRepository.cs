using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Common.CommonData;
using System.Linq.Expressions;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.BusinessLayer.Interfaces.Repositories.TripRepository;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Repository.TripRepository
{
    public class TripQueryRepository(AppDbContext context) : ITripQueryRepository
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

        public async Task<TripDetailsDto?> GetTripAsync(Expression<Func<Trip, bool>> predicate, CancellationToken cancellationToken)
            => await ProjectTripDetails(
                context.Trips.AsNoTracking()
                    .Where(predicate))
                .FirstOrDefaultAsync(cancellationToken);

        public async Task<PagedResult<TripDetailsDto>> FilterTripsAsync(FilterTripsDto filterDto, CancellationToken cancellationToken)
        {
            var query = context.Trips.AsNoTracking();

            if (filterDto.Id.HasValue)
            {
                query = query.Where(t => t.Id == filterDto.Id.Value);
            }

            if (filterDto.HandlerId.HasValue)
            {
                query = query.Where(t => t.Carrier.HandlerId == filterDto.HandlerId.Value);
            }

            if (filterDto.TripType.HasValue)
            {
                query = query.Where(t => t.TripType == filterDto.TripType.Value);
            }

            if (filterDto.Status is { Length: > 0 })
            {
                query = query.Where(t => filterDto.Status.Contains(t.Status));
            }

            if (filterDto.WarehouseId.HasValue)
            {
                query = query.Where(t => t.WarehouseId == filterDto.WarehouseId.Value);
            }

            if (filterDto.CreatedAt.HasValue)
            {
                var createdAt = filterDto.CreatedAt.Value.Date;
                query = query.Where(t => t.CreatedAt.Date == createdAt);
            }

            if (filterDto.From.HasValue)
            {
                query = query.Where(t => t.CreatedAt >= filterDto.From.Value);
            }

            if (filterDto.To.HasValue)
            {
                query = query.Where(t => t.CreatedAt <= filterDto.To.Value);
            }

            if (!string.IsNullOrWhiteSpace(filterDto.CreatedBy))
            {
                query = query.Where(t => t.CreatedBy == filterDto.CreatedBy);
            }

            if (filterDto.CarrierId.HasValue)
            {
                query = query.Where(t => t.CarrierId == filterDto.CarrierId.Value);
            }

            var totalCount = await query.CountAsync(cancellationToken);
            var pageNumber = filterDto.PageNumber <= 0 ? 1 : filterDto.PageNumber;
            var pageSize = filterDto.PageSize <= 0 ? 10 : filterDto.PageSize;

            var trips = await ProjectTripDetails(query)
                .OrderByDescending(t => t.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return PagedResult<TripDetailsDto>.From(trips, totalCount, pageNumber, pageSize);
        }

        private static IQueryable<TripDetailsDto> ProjectTripDetails(IQueryable<Trip> query)
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
                CreatedAt = t.CreatedAt,
                CreatedBy = t.CreatedBy,
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
                        Address = new AddressDto { MainAddress = sh.Receiver.Address.MainAddress, SecondaryAddress = sh.Receiver.Address.SecondaryAddress, Street = sh.Receiver.Address.Street }
                    },
                    Sender = new UserSummaryDto
                    {
                        FullName = sh.Sender.FullName,
                        Email = sh.Sender.Email,
                        PhoneNumber = sh.Sender.PhoneNumber,
                        Address = new AddressDto { MainAddress = sh.Sender.Address.MainAddress, SecondaryAddress = sh.Sender.Address.SecondaryAddress, Street = sh.Sender.Address.Street }
                    },
                    DeliveryAddress = new AddressDto { MainAddress = sh.DeliveryAddress.MainAddress, SecondaryAddress = sh.DeliveryAddress.SecondaryAddress, Street = sh.DeliveryAddress.Street },
                    PickupAddress = new AddressDto { MainAddress = sh.PickupAddress.MainAddress, SecondaryAddress = sh.PickupAddress.SecondaryAddress, Street = sh.PickupAddress.Street },
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
                    CreatedAt = sh.CreatedAt
                }).ToList()
            });

        private static IQueryable<CarrierTripDto> ProjectCarrierTripDetails(IQueryable<Trip> query)
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
                        Address = new AddressDto { MainAddress = sh.Receiver.Address.MainAddress, SecondaryAddress = sh.Receiver.Address.SecondaryAddress, Street = sh.Receiver.Address.Street }
                    },
                    Sender = new UserSummaryDto
                    {
                        FullName = sh.Sender.FullName,
                        Email = sh.Sender.Email,
                        PhoneNumber = sh.Sender.PhoneNumber,
                        Address = new AddressDto { MainAddress = sh.Sender.Address.MainAddress, SecondaryAddress = sh.Sender.Address.SecondaryAddress, Street = sh.Sender.Address.Street }
                    },
                    DeliveryAddress = new AddressDto { MainAddress = sh.DeliveryAddress.MainAddress, SecondaryAddress = sh.DeliveryAddress.SecondaryAddress, Street = sh.DeliveryAddress.Street },
                    PickupAddress = new AddressDto { MainAddress = sh.PickupAddress.MainAddress, SecondaryAddress = sh.PickupAddress.SecondaryAddress, Street = sh.PickupAddress.Street },
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
                    CreatedAt = sh.CreatedAt
                }).ToList()
            });
    }
}
