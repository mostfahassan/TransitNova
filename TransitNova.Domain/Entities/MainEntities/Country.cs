using TransitNova.Domain.Entities.Common;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class Country : BaseEntity<int>
    {
        public string Name { get; set; } = null!;
        public ICollection<Government> Governments { get; set; } = new List<Government>();

        private Country()
        {
            
        }
        private Country(string name)
        {
            Name = name;
            CurrentState = true;
            CreatedAt = DateTime.UtcNow;
        }
        public static Country Create(string name)
        {
          
            return new Country(name);
        }

        public void Update(string name)
        {
            Name = name;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
