using TransitNova.BusinessLayer.DTOs.Country;
namespace TransitNovaUI.BusinessLayer.DTOs.Country;

public sealed class UiGovernmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
    public string CountryName { get; set; } = string.Empty;

    public static UiGovernmentDto ToUiDto(GovernmentDto source) =>
        new()
        {
            Id = source.Id,
            Name = source.Name,
            CountryId = source.CountryId,
            CountryName = source.CountryName
        };
}
