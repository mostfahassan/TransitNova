namespace TransitNova.InfraStructure.Common.Exceptions;

public sealed class UserNotFoundException : IdentityOperationException
{
    public UserNotFoundException(Guid userId)
        : base($"User {userId} was not found.")
    {
    }
}
