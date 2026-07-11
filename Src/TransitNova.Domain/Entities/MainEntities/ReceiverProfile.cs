using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class ReceiverProfile : BaseInfo<Guid>
    {
        public virtual ICollection<Shipment> ReceivedShipments { get;} = new List<Shipment>();
        public UserProfile Sender { get; set; } = null!;
        public Guid SenderId { get; private set; }
        
        private ReceiverProfile()
        {

        }

        private ReceiverProfile(string firstName, string lastName, string email, string phone, Address address, int cityId, Guid senderId)
        {
            Id  = Guid.CreateVersion7();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phone;
            Address = address;
            UserType = UserType.Receiver;
            CityId = cityId;
            SenderId = senderId;
            CurrentState = true;
        }

        public static ReceiverProfile Create(string firstName, string lastName, string email, string phone, Address address, int cityId, Guid senderId)
        {
            return new ReceiverProfile(firstName, lastName, email, phone, address, cityId, senderId);

        }
    }
}   