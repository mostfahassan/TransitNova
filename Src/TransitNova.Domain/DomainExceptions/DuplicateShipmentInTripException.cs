namespace TransitNova.Domain.DomainExceptions
{

    public class DuplicateShipmentInTripException : DomainException
    {
        public DuplicateShipmentInTripException(string message, string errorCode = "SHIPMENT_ALREADY_EXISTS_IN_TRIP", string? entityType = null, Guid? entityId = null)
            : base(message, errorCode, entityType, entityId) { }
    }

}
