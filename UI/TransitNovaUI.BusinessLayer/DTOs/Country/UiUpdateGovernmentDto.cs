using TransitNova.BusinessLayer.DTOs.Country;
namespace TransitNovaUI.BusinessLayer.DTOs.Country;

public sealed class UiUpdateGovernmentDto
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }

    public static UpdateGovernmentDto ToDto(UiUpdateGovernmentDto source) =>
        new() { Name = source.Name, CountryId = source.CountryId };

}
