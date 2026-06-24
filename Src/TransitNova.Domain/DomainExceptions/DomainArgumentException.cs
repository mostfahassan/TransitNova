namespace TransitNova.Domain.DomainExceptions
{

    public class DomainArgumentException : DomainException
    {
        public string? ParamName { get; }
        public DomainArgumentException(string paramName, string message, string errorCode = "INVALID_ARGUMENT", string? entityType = null, Guid? entityId = null)
            : base(message, errorCode, entityType, entityId)
        {
            ParamName = paramName;
        }
    }

}
