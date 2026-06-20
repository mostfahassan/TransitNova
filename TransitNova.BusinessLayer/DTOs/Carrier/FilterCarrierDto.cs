using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.BusinessLayer.DTOs.Carrier
{
    public class FilterCarrierDto
    {
        public CarrierStatus? Status { get; set; }
        public decimal? MinRating { get; set; }
        public decimal? MaxRating { get; set; }   
        public int? MinYearsOfExperience { get; set; }
        public int? MaxYearsOfExperience { get; set; }
        public Guid? CompanyId { get; set; }
        public string? City { get; set; }
        public int? CityId { get; set; }
        public string? SearchTerm { get; set; }
        public DateTime? AvailableFrom { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public CarrierSortBy? SortBy { get; set; } 
        public bool SortDescending { get; set; } = false;
        public decimal? VehicleCapacityWeight { get; set; }
        public VehicleType? VehicleType { get; set; }
        public List<string>? ServedZones { get; set; }
    }
}
