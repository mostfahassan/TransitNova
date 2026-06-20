namespace TransitNova.BusinessLayer.DTOs.City
{
    public class CityDto: IBaseLocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int GovernmentId { get; set; }
        public string GovernmentName { get; set; } = string.Empty;
    }
    public interface IBaseLocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}

