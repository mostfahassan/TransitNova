namespace TransitNova.InfraStructure.Common.Exceptions;

public sealed class UserCreationException : IdentityOperationException
{
    public UserCreationException(string errors)
        : base($"User creation failed. Errors: {errors}")
    {
    }
}
