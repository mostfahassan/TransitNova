using System;
using System.Collections.Generic;
using System.Text;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents
{
    public sealed record TripShipmentAddedDomainEvent(Guid Id, Guid ShipmentId, ShipmentStatuses ShipmentStatus, int TotalShipments) : IDomainEvent;

}
