using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.Domain.Enums.Users;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
namespace TransitNovaUI.BusinessLayer.DTOs.UserProfile;

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
