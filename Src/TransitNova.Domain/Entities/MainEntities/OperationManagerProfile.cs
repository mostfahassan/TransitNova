
using TransitNova.Domain.Contracts.DomainEvents.Events.UsersEvent;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class OperationManagerProfile : BaseInfo<Guid>
    {
        private readonly List<Carrier> _handledCarriers = new();
        private readonly List<Shipment> _handledShipments = new();
        public Guid AppUserId { get; private set; }
        public IReadOnlyCollection<Shipment> HandledShipments => _handledShipments;
        public IReadOnlyCollection<Carrier> HandledCarriers => _handledCarriers;
        private OperationManagerProfile()
        {
        }
        private OperationManagerProfile(Guid appUserId, string firstName, string lastName, string email, string phoneNumber, Address address, int cityId)
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
        public static OperationManagerProfile Create(Guid id, string firstName, string lastName, string email, string phoneNumber, Address address, int cityId)
        {
            var user = new OperationManagerProfile(id, firstName, lastName, email, phoneNumber, address, cityId);
            user.RaiseDomainEvent(new UserRegisteredDomainEvent(user.Id, email, user.FullName,phoneNumber, user.UserType));
            return user;
        }

    }
}   
