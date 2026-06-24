namespace TransitNova.InfraStructure.Common.Exceptions;

public abstract class IdentityOperationException : Exception
{
    protected IdentityOperationException(string message)
        : base(message)
    {
    }
}
