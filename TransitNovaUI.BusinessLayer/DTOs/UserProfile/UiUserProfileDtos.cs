using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.Domain.Enums.Users;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;

namespace TransitNovaUI.BusinessLayer.DTOs.UserProfile;

public class UiUserProfileDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public UserType UserType { get; set; }
    public string CityName { get; set; } = string.Empty;
    public string GovernmentName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public int TotalShipmentsSent { get; set; }
    public string? BundleName { get; set; }

    public static UiUserProfileDto ToUiDto(UserProfileDto source) =>
        new()
        {
            Id = source.Id,
            FullName = source.FullName,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            Address = source.Address,
            UserType = source.UserType,
            CityName = source.CityName,
            GovernmentName = source.GovernmentName,
            CountryName = source.CountryName,
            TotalShipmentsSent = source.TotalShipmentsSent,
            BundleName = source.BundleName
        };
}

public sealed class UiAdminUserDetailsDto : UiUserProfileDto
{
    public Guid UserId { get; set; }
    public Guid ProfileId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public bool IsLockedOut { get; set; }
    public DateTime CreatedAt { get; set; }

    public static UiAdminUserDetailsDto ToUiDto(AdminUserDetailsDto source) =>
        new()
        {
            Id = source.Id,
            FullName = source.FullName,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            Address = source.Address,
            UserType = source.UserType,
            CityName = source.CityName,
            GovernmentName = source.GovernmentName,
            CountryName = source.CountryName,
            TotalShipmentsSent = source.TotalShipmentsSent,
            BundleName = source.BundleName,
            UserId = source.UserId,
            ProfileId = source.ProfileId,
            UserName = source.UserName,
            IsActive = source.IsActive,
            IsLockedOut = source.IsLockedOut,
            CreatedAt = source.CreatedAt
        };

    public static UiPagedResult<UiAdminUserDetailsDto> ToUiPagedDto(PagedResult<AdminUserDetailsDto> source) =>
        UiPagedResult<UiAdminUserDetailsDto>.From(
            source.Data.Select(ToUiDto),
            source.TotalCount,
            source.PageNumber,
            source.PageSize);
}

public sealed class UiUserFiltrationDto
{
    public string? SearchTerm { get; set; }
    public string? Email { get; set; }
    public string? UserName { get; set; }
    public string? PhoneNumber { get; set; }
    public bool? IsActive { get; set; }
    public DateTime? CreatedFrom { get; set; }
    public DateTime? CreatedTo { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public static UserFiltrationDto ToDto(UiUserFiltrationDto source) =>
        new()
        {
            SearchTerm = source.SearchTerm,
            Email = source.Email,
            UserName = source.UserName,
            PhoneNumber = source.PhoneNumber,
            IsActive = source.IsActive,
            CreatedFrom = source.CreatedFrom,
            CreatedTo = source.CreatedTo,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize
        };

}

public sealed class UiUserSummaryDto
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;

    public static UiUserSummaryDto ToUiDto(UserSummaryDto source) =>
        new()
        {
            FullName = source.FullName,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            Address = source.Address,
            CityName = source.CityName
        };
}
