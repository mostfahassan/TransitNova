using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.Domain.Enums.Carrier;

namespace TransitNovaUI.BusinessLayer.DTOs.Carrier;

public sealed class UiFilterCarrierDto
{
    public CarrierStatus? Status { get; set; }
    public decimal? MinRating { get; set; }
    public decimal? MaxRating { get; set; }
    public int? MinYearsOfExperience { get; set; }
    public int? MaxYearsOfExperience { get; set; }
    public string? City { get; set; }
    public int? CityId { get; set; }
    public string? SearchTerm { get; set; }
    public DateTime? AvailableFrom { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;
    public CarrierSortBy? SortBy { get; set; }
    public bool SortDescending { get; set; }
    public decimal? VehicleCapacityWeight { get; set; }
    public VehicleType? VehicleType { get; set; }
    public List<string>? ServedZones { get; set; }

    public static FilterCarrierDto ToDto(UiFilterCarrierDto source) =>
        new()
        {
            Status = source.Status,
            MinRating = source.MinRating,
            MaxRating = source.MaxRating,
            MinYearsOfExperience = source.MinYearsOfExperience,
            MaxYearsOfExperience = source.MaxYearsOfExperience,
       
            City = source.City,
            CityId = source.CityId,
            SearchTerm = source.SearchTerm,
            AvailableFrom = source.AvailableFrom,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize,
            SortBy = source.SortBy,
            SortDescending = source.SortDescending,
            VehicleCapacityWeight = source.VehicleCapacityWeight,
            VehicleType = source.VehicleType,
            ServedZones = source.ServedZones is null ? null : [.. source.ServedZones]
        };

}
