namespace TransitNova.BusinessLayer.DTOs.City
{
    public class CityFilterDto
    {
        public int? GovernmentId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool SortDescending { get; set; }
    }
}

