namespace TransitNova.Domain.DomainExceptions
{

    public class EntityNotFoundException : DomainException
    {
        public EntityNotFoundException(string message, string errorCode = "ENTITY_NOT_FOUND", string? entityType = null, Guid? entityId = null)
            : base(message, errorCode, entityType, entityId) { }
    }

}
