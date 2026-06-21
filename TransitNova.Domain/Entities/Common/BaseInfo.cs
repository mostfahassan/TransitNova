using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.Domain.Entities.Common
{
    public class BaseInfo<TKey> : AggregateRoot<TKey>
    {
        public string FirstName { get;protected set; } = string.Empty;
        public string LastName { get; protected set; } = string.Empty;
        public string FullName => $"{FirstName} {LastName}".Trim();
        public string Email { get; protected set; } = string.Empty;
        public string PhoneNumber { get; protected set; } = string.Empty;
        public string Address { get; protected set; } = string.Empty;
        public UserType UserType { get; protected set; }
        public int CityId { get; protected set; }
        public City City { get; set; } = null!;

    }

 


}
