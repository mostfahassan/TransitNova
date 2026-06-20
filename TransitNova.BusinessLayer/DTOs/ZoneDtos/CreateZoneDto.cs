namespace TransitNova.BusinessLayer.DTOs.ZoneDtos
{
    public class CreateZoneDto
    {
        public string Name { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public int CityId { get; set; }
    }
}

