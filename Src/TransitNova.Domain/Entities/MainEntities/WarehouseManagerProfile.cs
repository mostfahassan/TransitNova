
using TransitNova.Domain.Contracts.DomainEvents.Events.UsersEvent;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class WarehouseManagerProfile : BaseInfo<Guid>
    {  
        public Guid AppUserId { get; private set; }
        public Warehouse? Warehouse { get; private set; }
        private WarehouseManagerProfile(Guid appUserId, string firstName, string lastName, string email, string phoneNumber, string address, int cityId)
        {
            Id = Guid.CreateVersion7();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phoneNumber;
            Address = address;
            AppUserId = appUserId;
            UserType = UserType.WarehouseManager;
            CurrentState = true;
            CityId = cityId;
        }
        public static WarehouseManagerProfile Create(Guid id, string firstName, string lastName, string email, string phoneNumber, string address, int cityId)
        {
            var user = new WarehouseManagerProfile(id, firstName, lastName, email, phoneNumber, address, cityId);
            user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id, email, user.FullName,phoneNumber, user.UserType));
            return user;
        }

        public void Update(string? firstName = null, string? lastName = null, string? phoneNumber = null, string? email = null, int? cityId = null,
            string? address = null)
        {

            FirstName = firstName ?? FirstName;
            LastName = lastName ?? LastName;
            PhoneNumber = phoneNumber ?? PhoneNumber;
            Email = email ?? Email;
            CityId = cityId ?? CityId;
            Address = address ?? Address;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = Id.ToString();
        }

    }
}   