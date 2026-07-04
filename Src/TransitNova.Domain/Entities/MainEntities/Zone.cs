using TransitNova.Domain.Entities.Common;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class Zone : BaseEntity<Guid>
    {
        public string Name { get; private set; } = string.Empty;
        public string Code { get; private set; } = string.Empty;
        public City City { get; set; } = null!; 
        public int CityId { get; private set; }
        public ICollection<Warehouse> Warehouses { get; set; } = new List<Warehouse>();
        public virtual ICollection<Carrier> ServedByCarriers { get; set; } = new List<Carrier>();
        public void Update(string? name, string? code, int cityId)
        {
            Name = name?.Trim() ?? string.Empty;
            Code = code?.Trim() ?? string.Empty;
            CityId = cityId;
            UpdatedAt = DateTime.UtcNow;
        }
        private Zone()
        {
        }
         private Zone(string name,int cityId)
        {
            Id = Guid.CreateVersion7();
            Name = name;
            Code = GenerateZoneCode(name);
            CityId = cityId;
            CurrentState = true;
        }

        public static Zone Create(string name,int cityId)
            => new (name, cityId);




        static string GenerateZoneCode(string cityName)
        {
            if (string.IsNullOrWhiteSpace(cityName))
                throw new ArgumentException("City name cannot be empty.", nameof(cityName));

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            var prefix = new string([..
                  cityName.Trim()
                  .Where(char.IsLetter)
                  .Take(3)
                  .Select(char.ToUpperInvariant)]);



            prefix = prefix.PadRight(3, 'X');

            Span<char> randomPart = stackalloc char[4];

            for (var i = 0; i < randomPart.Length; i++)
            {
                randomPart[i] = chars[Random.Shared.Next(chars.Length)];
            }

            return $"{prefix}-{new string(randomPart)}";
        }
    }
}

