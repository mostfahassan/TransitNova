using System;
namespace TransitNova.Domain.DomainExceptions
{

    public sealed class InvalidShipmentStateException : DomainException
    {
        public InvalidShipmentStateException(Guid shipmentId, string expected, string actual)
            : base($"Shipment {shipmentId} is in {actual} state. Expected: {expected}", "INVALID_SHIPMENT_STATE", "Shipment", shipmentId) { }
    }

}
