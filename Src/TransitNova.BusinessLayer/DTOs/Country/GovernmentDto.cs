using TransitNova.BusinessLayer.DTOs.City;
namespace TransitNova.BusinessLayer.DTOs.Country
{
    public class GovernmentDto: IBaseLocationDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

}
