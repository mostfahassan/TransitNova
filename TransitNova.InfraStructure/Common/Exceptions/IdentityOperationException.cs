namespace TransitNova.InfraStructure.Common.Exceptions;

public abstract class IdentityOperationException : Exception
{
    protected IdentityOperationException(string message)
        : base(message)
    {
    }
}

public sealed class UserCreationException : IdentityOperationException
{
    public UserCreationException(string errors)
        : base($"User creation failed. Errors: {errors}")
    {
    }
}

public sealed class UserNotFoundException : IdentityOperationException
{
    public UserNotFoundException(Guid userId)
        : base($"User {userId} was not found.")
    {
    }
}

public sealed class RoleAssignmentException : IdentityOperationException
{
    public RoleAssignmentException(Guid userId, string roleName, string errors)
        : base($"Failed to add user {userId} to role '{roleName}'. Errors: {errors}")
    {
    }
}

public sealed class PasswordChangeException : IdentityOperationException
{
    public PasswordChangeException(Guid userId, string errors)
        : base($"Password change failed for user {userId}. Errors: {errors}")
    {
    }
}

public sealed class UserDeletionException : IdentityOperationException
{
    public UserDeletionException(Guid userId, string errors)
        : base($"Failed to delete user {userId}. Errors: {errors}")
    {
    }
}
