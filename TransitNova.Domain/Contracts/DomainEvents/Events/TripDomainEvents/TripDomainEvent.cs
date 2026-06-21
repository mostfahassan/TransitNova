using System;
using System.Collections.Generic;
using System.Text;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;

namespace TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents
{
    public sealed record TripPlannedDomainEvent(Guid Id, Guid CarrierId, Guid WarehouseId, TripType TripType, int TotalShipments) : IDomainEvent;
    public sealed record TripUpdatedDomainEvent(Guid Id, TripType TripType, TripStatus Status, int TotalShipments) : IDomainEvent;
    public sealed record TripStartedDomainEvent(Guid Id, TripType TripType, DateTime StartTime) : IDomainEvent;
    public sealed record TripCompletedDomainEvent(Guid Id, Guid CarrierId, TripStatus Status, DateTime EndTime, int TotalShipments) : IDomainEvent;
    public sealed record TripCancelledDomainEvent(Guid Id, TripStatus Status, DateTime CancelledAt) : IDomainEvent;
    public sealed record TripShipmentAddedDomainEvent(Guid Id, Guid ShipmentId, ShipmentStatuses ShipmentStatus, int TotalShipments) : IDomainEvent;

}
