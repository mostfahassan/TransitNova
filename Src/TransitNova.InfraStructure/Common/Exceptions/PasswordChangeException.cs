namespace TransitNova.InfraStructure.Common.Exceptions;

public sealed class PasswordChangeException : IdentityOperationException
{
    public PasswordChangeException(Guid userId, string errors)
        : base($"Password change failed for user {userId}. Errors: {errors}")
    {
    }
}
