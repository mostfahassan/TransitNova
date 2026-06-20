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

public sealed class UiCityFilterDto
{
    public int? GovernmentId { get; set; }
    public string? SearchTerm { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public bool SortDescending { get; set; }

    public static CityFilterDto ToDto(UiCityFilterDto source) =>
        new()
        {
            GovernmentId = source.GovernmentId,
            SearchTerm = source.SearchTerm,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize,
            SortDescending = source.SortDescending
        };

}

public sealed class UiCreateCityDto
{
    public string Name { get; set; } = string.Empty;
    public int GovernmentId { get; set; }

    public static CreateCityDto ToDto(UiCreateCityDto source) =>
        new() { Name = source.Name, GovernmentId = source.GovernmentId };

}

public sealed class UiUpdateCityDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int GovernmentId { get; set; }

    public static UpdateCityDto ToDto(UiUpdateCityDto source) =>
        new()
        {
            Id = source.Id,
            Name = source.Name,
            GovernmentId = source.GovernmentId
        };

}
