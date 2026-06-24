using FluentValidation;
using TransitNova.BusinessLayer.DTOs.UserProfile;
namespace TransitNova.BusinessLayer.Validators.UserProfile.User;
public class UserFiltrationDtoValidator : AbstractValidator<UserFiltrationDto>
{
    private const int MaxPageSize = 100;
    private const int MaxSearchTermLength = 100;
    private const int MaxEmailLength = 256;
    private const int MaxUserNameLength = 50;
    private const int MaxPhoneLength = 20;

    public UserFiltrationDtoValidator()
    {
        // ── Pagination ──────────────────────────────────────────────────────
        RuleFor(x => x.PageNumber)
            .GreaterThanOrEqualTo(1)
            .WithMessage("Page number must be at least 1.");

        RuleFor(x => x.PageSize)
            .InclusiveBetween(1, MaxPageSize)
            .WithMessage($"Page size must be between 1 and {MaxPageSize}.");

        // ── Search Term ─────────────────────────────────────────────────────
        RuleFor(x => x.SearchTerm)
            .NotEmpty()
            .WithMessage("Search term must not be empty or whitespace.")
            .MaximumLength(MaxSearchTermLength)
            .WithMessage($"Search term must not exceed {MaxSearchTermLength} characters.")
            .When(x => x.SearchTerm is not null);

        // ── Email ───────────────────────────────────────────────────────────
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email must not be empty or whitespace.")
            .MaximumLength(MaxEmailLength)
            .WithMessage($"Email must not exceed {MaxEmailLength} characters.")
            .EmailAddress()
            .WithMessage("Email format is invalid.")
            .When(x => x.Email is not null);

        // ── Username ────────────────────────────────────────────────────────
        RuleFor(x => x.UserName)
            .NotEmpty()
            .WithMessage("Username must not be empty or whitespace.")
            .MaximumLength(MaxUserNameLength)
            .WithMessage($"Username must not exceed {MaxUserNameLength} characters.")
            .Matches(@"^[a-zA-Z0-9._\-]+$")
            .WithMessage("Username may only contain letters, digits, dots, underscores, and hyphens.")
            .When(x => x.UserName is not null);

        // ── Phone Number ────────────────────────────────────────────────────
        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Phone number must not be empty or whitespace.")
            .MaximumLength(MaxPhoneLength)
            .WithMessage($"Phone number must not exceed {MaxPhoneLength} characters.")
            .Matches(@"^\+?[0-9\s\-\(\)]+$")
            .WithMessage("Phone number format is invalid.")
            .When(x => x.PhoneNumber is not null);

        // ── Mutual Exclusivity: SearchTerm vs specific fields ───────────────
        RuleFor(x => x)
            .Must(x => x.SearchTerm is null || (x.Email is null && x.UserName is null && x.PhoneNumber is null))
            .WithName("Search Conflict")
            .WithMessage("Cannot combine 'SearchTerm' with 'Email', 'UserName', or 'PhoneNumber' filters.");

        // ── Date Range ──────────────────────────────────────────────────────
        RuleFor(x => x.CreatedFrom)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("'CreatedFrom' date must not be in the future.")
            .When(x => x.CreatedFrom.HasValue);

        RuleFor(x => x.CreatedTo)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("'CreatedTo' date must not be in the future.")
            .When(x => x.CreatedTo.HasValue);

        RuleFor(x => x)
            .Must(x => x.CreatedFrom <= x.CreatedTo)
            .WithName("Date Range")
            .WithMessage("'CreatedFrom' must not be after 'CreatedTo'.")
            .When(x => x.CreatedFrom.HasValue && x.CreatedTo.HasValue);
    }
}
