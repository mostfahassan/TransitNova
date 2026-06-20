namespace TransitNova.BusinessLayer.DTOs.City
{
    public class UpdateCityDto : IBaseLocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GovernmentId { get; set; }
    }
}

