namespace TransitNova.InfraStructure.Common.Exceptions;

public sealed class UserDeletionException : IdentityOperationException
{
    public UserDeletionException(Guid userId, string errors)
        : base($"Failed to delete user {userId}. Errors: {errors}")
    {
    }
}
