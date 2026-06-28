
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
            UserType = UserType.OperationManager;
            CurrentState = true;
            CityId = cityId;
        }
        public static WarehouseManagerProfile Create(Guid id, string firstName, string lastName, string email, string phoneNumber, string address, int cityId)
        {
            var user = new WarehouseManagerProfile(id, firstName, lastName, email, phoneNumber, address, cityId);
            user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id, email, user.FullName,phoneNumber, user.UserType));
            return user;
        }

    }
}   