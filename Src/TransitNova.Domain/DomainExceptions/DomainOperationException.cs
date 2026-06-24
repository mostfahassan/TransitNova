namespace TransitNova.Domain.DomainExceptions
{
    public class DomainOperationException : DomainException
    {
        public DomainOperationException(string message, string errorCode = "INVALID_OPERATION", string? entityType = null, Guid? entityId = null)
            : base(message, errorCode, entityType, entityId) { }
    }

}
