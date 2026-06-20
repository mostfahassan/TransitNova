using TransitNova.BusinessLayer.DTOs.CarrierCompany;

namespace TransitNovaUI.BusinessLayer.DTOs.CarrierCompany;

public sealed class UiAddCarrierCompany
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Guid? ZoneId { get; set; }

    public static AddCarrierCompany ToDto(UiAddCarrierCompany source) =>
        new()
        {
            Name = source.Name,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            Address = source.Address,
            ZoneId = source.ZoneId
        };

}

public sealed class UiRetrieveCarrierCompany
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Guid? ZoneId { get; set; }
    public string Code { get; set; } = string.Empty;
    public string CityName { get; set; } = string.Empty;
    public string CountryName { get; set; } = string.Empty;
    public DateTime ContractStartDate { get; set; }
    public DateTime ContractEndDate { get; set; }

    public static UiRetrieveCarrierCompany ToUiDto(RetrieveCarrierCompany source) =>
        new()
        {
            Name = source.Name,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            Address = source.Address,
            ZoneId = source.ZoneId,
            Code = source.Code,
            CityName = source.CityName,
            CountryName = source.CountryName,
            ContractStartDate = source.ContractStartDate,
            ContractEndDate = source.ContractEndDate
        };
}

public sealed class UiUpdateCarrierCompany
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public Guid? ZoneId { get; set; }

    public static UpdateCarrierCompany ToDto(UiUpdateCarrierCompany source) =>
        new()
        {
            Id = source.Id,
            Name = source.Name,
            Email = source.Email,
            PhoneNumber = source.PhoneNumber,
            Address = source.Address,
            ZoneId = source.ZoneId
        };

}
