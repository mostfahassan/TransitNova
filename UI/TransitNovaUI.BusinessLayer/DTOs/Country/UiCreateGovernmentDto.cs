using TransitNova.BusinessLayer.DTOs.Country;
namespace TransitNovaUI.BusinessLayer.DTOs.Country;

public sealed class UiCreateGovernmentDto
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }

    public static CreateGovernmentDto ToDto(UiCreateGovernmentDto source) =>
        new() { Name = source.Name, CountryId = source.CountryId };

}
