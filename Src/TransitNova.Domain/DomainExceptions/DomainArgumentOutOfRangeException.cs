namespace TransitNova.Domain.DomainExceptions
{

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
