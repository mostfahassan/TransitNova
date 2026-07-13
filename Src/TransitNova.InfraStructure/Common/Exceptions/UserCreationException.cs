using Microsoft.AspNetCore.Identity;

namespace TransitNova.InfraStructure.Common.Exceptions;

public sealed class UserCreationException : IdentityOperationException
{
    private static readonly HashSet<string> DuplicateErrorCodes =
    [
        nameof(IdentityErrorDescriber.DuplicateEmail),
        nameof(IdentityErrorDescriber.DuplicateUserName)
    ];

    public UserCreationException(IEnumerable<IdentityError> errors)
        : this(errors.Select(error => error.Code), errors.Select(error => error.Description))
    {
    }

    public UserCreationException(string errors)
        : this([], [errors])
    {
    }

    private UserCreationException(
        IEnumerable<string> errorCodes,
        IEnumerable<string> descriptions)
        : base($"User creation failed. Errors: {string.Join(", ", descriptions)}")
    {
        ErrorCodes = errorCodes
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Distinct(StringComparer.Ordinal)
            .ToArray();
    }

    public IReadOnlyCollection<string> ErrorCodes { get; }

    public bool IsDuplicate => ErrorCodes.Any(DuplicateErrorCodes.Contains);
}
