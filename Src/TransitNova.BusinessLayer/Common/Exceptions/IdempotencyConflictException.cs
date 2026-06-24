namespace TransitNova.BusinessLayer.Common.Exceptions
{
    public sealed class IdempotencyConflictException : Exception
    {
        public IdempotencyConflictException()
            : base("The request has already been processed.") { }
    }
}
