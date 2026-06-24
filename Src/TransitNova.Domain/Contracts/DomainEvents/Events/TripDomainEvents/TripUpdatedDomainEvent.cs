using System;
using System.Collections.Generic;
using System.Text;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents
{
    public sealed record TripUpdatedDomainEvent(Guid Id, TripType TripType, TripStatus Status, int TotalShipments) : IDomainEvent;

}
