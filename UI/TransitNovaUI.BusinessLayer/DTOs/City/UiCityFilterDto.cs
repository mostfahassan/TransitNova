using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
namespace TransitNovaUI.BusinessLayer.DTOs.City;

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
