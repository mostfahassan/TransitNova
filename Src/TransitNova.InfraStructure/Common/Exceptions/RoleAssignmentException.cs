namespace TransitNova.InfraStructure.Common.Exceptions;

public sealed class RoleAssignmentException : IdentityOperationException
{
    public RoleAssignmentException(Guid userId, string roleName, string errors)
        : base($"Failed to add user {userId} to role '{roleName}'. Errors: {errors}")
    {
    }
}
