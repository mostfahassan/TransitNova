using TransitNova.BusinessLayer.DTOs.Country;

namespace TransitNovaUI.BusinessLayer.DTOs.Country;

public sealed class UiCountryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public static UiCountryDto ToUiDto(CountryDto source) =>
        new() { Id = source.Id, Name = source.Name };
}

public sealed class UiGovernmentDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public static UiGovernmentDto ToUiDto(GovernmentDto source) =>
        new() { Id = source.Id, Name = source.Name };
}

public sealed class UiCreateGovernmentDto
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }

    public static CreateGovernmentDto ToDto(UiCreateGovernmentDto source) =>
        new() { Name = source.Name, CountryId = source.CountryId };

}

public sealed class UiUpdateGovernmentDto
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }

    public static UpdateGovernmentDto ToDto(UiUpdateGovernmentDto source) =>
        new() { Name = source.Name, CountryId = source.CountryId };

}
