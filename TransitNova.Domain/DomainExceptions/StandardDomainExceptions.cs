
namespace TransitNova.Domain.DomainExceptions
{
    public class DomainOperationException : DomainException
    {
        public DomainOperationException(string message, string errorCode = "INVALID_OPERATION", string? entityType = null, Guid? entityId = null)
            : base(message, errorCode, entityType, entityId) { }
    }

    public class DuplicateShipmentInTripException : DomainException
    {
        public DuplicateShipmentInTripException(string message, string errorCode = "SHIPMENT_ALREADY_EXISTS_IN_TRIP", string? entityType = null, Guid? entityId = null)
            : base(message, errorCode, entityType, entityId) { }
    }

    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string message, string errorCode = "ENTITY_NOT_FOUND", string? entityType = null, Guid? entityId = null)
            : base(message, errorCode, entityType, entityId) { }
    }

    public class DomainArgumentException : DomainException
    {
        public string? ParamName { get; }
        public DomainArgumentException(string paramName, string message, string errorCode = "INVALID_ARGUMENT", string? entityType = null, Guid? entityId = null)
            : base(message, errorCode, entityType, entityId)
        {
            ParamName = paramName;
        }
    }

   public class DomainArgumentOutOfRangeException : DomainException
    {
        public string? ParamName { get; }
        public DomainArgumentOutOfRangeException(string paramName, string message, string errorCode = "ARG_OUT_OF_RANGE", string? entityType = null, Guid? entityId = null)
            : base(message, errorCode, entityType, entityId)
        {
            ParamName = paramName;
        }
    }
}
