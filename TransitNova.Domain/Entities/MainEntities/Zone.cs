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
        public void Update(string name, string code, int cityId)
        {
            Name = name.Trim();
            Code = code.Trim();
            CityId = cityId;
            UpdatedAt = DateTime.UtcNow;
        }
        private Zone()
        {
        }
         private Zone(string name, string code, int cityId)
        {
            Id = Guid.CreateVersion7();
            Name = name;
            Code = code;
            CityId = cityId;
            CreatedAt = DateTime.UtcNow;
            CurrentState = true;
        }

        public static Zone Create(string name, string code, int cityId)
            => new (name, code, cityId);
        
    }
}

