using System;
using System.Collections.Generic;
using System.Text;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.TripDomainEvents
{
    public sealed record TripPlannedDomainEvent(Guid Id, Guid CarrierId, Guid WarehouseId, TripType TripType, int TotalShipments) : IDomainEvent;

}
