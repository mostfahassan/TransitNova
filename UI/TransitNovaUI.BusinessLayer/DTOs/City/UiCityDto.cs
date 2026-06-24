using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
namespace TransitNovaUI.BusinessLayer.DTOs.City;

public sealed class UiCityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GovernmentId { get; set; }
    public string GovernmentName { get; set; } = string.Empty;

    public static UiCityDto ToUiDto(CityDto source) =>
        new()
        {
            Id = source.Id,
            Name = source.Name,
            GovernmentId = source.GovernmentId,
            GovernmentName = source.GovernmentName
        };

    public static UiPagedResult<UiCityDto> ToUiPagedDto(PagedResult<CityDto> source) =>
        UiPagedResult<UiCityDto>.From(
            source.Data.Select(ToUiDto),
            source.TotalCount,
            source.PageNumber,
            source.PageSize);
}
