namespace TransitNova.Domain.DomainExceptions
{
    public sealed class ConflictRequestException : Exception
    {
        public ConflictRequestException(string message)
       : base(message)
        {
        }

    }
}
