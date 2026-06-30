namespace TransitNova.BusinessLayer.DTOs.ZoneDtos
{
    public class UpdateZoneDto
    {
        public Guid ZoneId { get; set; }
        public string? Name { get; set; } 
        public string? Code { get; set; } 
        public int CityId { get; set; }
    }
}

