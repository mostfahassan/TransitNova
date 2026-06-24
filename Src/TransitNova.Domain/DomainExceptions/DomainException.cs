using System;
namespace TransitNova.Domain.DomainExceptions
{
    public abstract class DomainException : Exception
    {
        public string ErrorCode { get; }
        public string? EntityType { get; }
        public Guid? EntityId { get; }

        protected DomainException(string message, string errorCode, string? entityType = null, Guid? entityId = null) : base(message)
        {
            ErrorCode = errorCode;
            EntityType = entityType;
            EntityId = entityId;
        }
    }

}
