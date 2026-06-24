using System;
using System.Collections.Generic;
using System.Text;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents
{
    public sealed record TripCompletedDomainEvent(Guid Id, Guid CarrierId, TripStatus Status, DateTime EndTime, int TotalShipments) : IDomainEvent;

}
