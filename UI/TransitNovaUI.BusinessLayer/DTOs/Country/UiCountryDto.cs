using TransitNova.BusinessLayer.DTOs.Country;
namespace TransitNovaUI.BusinessLayer.DTOs.Country;

public sealed class UiCountryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public static UiCountryDto ToUiDto(CountryDto source) =>
        new() { Id = source.Id, Name = source.Name };
}
