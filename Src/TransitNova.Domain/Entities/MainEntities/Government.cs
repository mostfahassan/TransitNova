
using TransitNova.Domain.Entities.Common;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class Government:BaseEntity<int>
    {
        public int CountryId { get; private set; }
        public Country Country { get; set; } = null!;
        public string Name { get; private set; } = string.Empty;
        public List<City> Cities { get; set; } = new List<City>();

        private Government()
        {
            

        }

        private Government(string name, int countryId)
        {
            Name = name;
            CountryId = countryId;
            CurrentState = true;

        }
        public static Government Create(string name, int countryId)
             => new (name, countryId);


        public void Update(string name, int countryId)
        {
            Name = name;
            CountryId = countryId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
