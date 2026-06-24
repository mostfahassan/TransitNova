using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNovaUI.BusinessLayer.Common.ResultPattern;
namespace TransitNovaUI.BusinessLayer.DTOs.City;

public sealed class UiUpdateCityDto
{
    public string Name { get; set; } = string.Empty;
    public int GovernmentId { get; set; }

    public static UpdateCityDto ToDto(UiUpdateCityDto source) =>
        new()
        {
            Name = source.Name,
            GovernmentId = source.GovernmentId
        };

}
