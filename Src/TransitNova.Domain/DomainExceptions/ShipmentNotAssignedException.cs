using System;
namespace TransitNova.Domain.DomainExceptions
{

    public class ShipmentNotAssignedException : DomainException
    {
        public ShipmentNotAssignedException(Guid shipmentId , Guid carrierId)
            : base($"Shipment {shipmentId} is not assigned to carrier {carrierId}.", "SHIPMENT_NOT_ASSIGNED_EXCEPTION") { }
    }

}
