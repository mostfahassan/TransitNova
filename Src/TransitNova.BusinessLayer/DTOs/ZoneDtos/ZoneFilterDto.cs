namespace TransitNova.BusinessLayer.DTOs.ZoneDtos
{
    public class ZoneFilterDto
    {
        public int? CityId { get; set; }
        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public bool SortDescending { get; set; }
    }
}

