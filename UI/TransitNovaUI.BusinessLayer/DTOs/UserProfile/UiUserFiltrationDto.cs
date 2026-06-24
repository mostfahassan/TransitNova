using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.Domain.Enums.Users;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
namespace TransitNovaUI.BusinessLayer.DTOs.UserProfile;

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
