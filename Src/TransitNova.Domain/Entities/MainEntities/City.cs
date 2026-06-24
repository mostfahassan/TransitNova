using TransitNova.Domain.Entities.Common;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class City : BaseEntity<int>
    {
        public string Name { get; private set; } = string.Empty;
        public Government Government { get;} = null!;
        public int GovernmentId { get; private set; }

        private readonly List<Zone> _zones = new();
        public IReadOnlyCollection<Zone> Zones => _zones;

        private City()
        {
            
        }

        private City(string name, int governmentId)
        {
            Name = name;
            GovernmentId = governmentId;
            CurrentState = true;
        
        }
        public static City Create(string name, int governmentId)
           =>new (name, governmentId);
       
        public void Update(string name, int governmentId)
        {
            Name = name;
            GovernmentId = governmentId;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
