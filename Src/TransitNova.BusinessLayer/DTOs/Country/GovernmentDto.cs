using TransitNova.BusinessLayer.DTOs.City;
namespace TransitNova.BusinessLayer.DTOs.Country
{
    public class GovernmentDto: IBaseLocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int CountryId { get; set; }
        public string CountryName { get; set; } = string.Empty;
    }

}
