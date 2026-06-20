using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class ReceiverProfile : BaseInfo
    {
        public virtual ICollection<Shipment> ReceivedShipments { get; set; } = new List<Shipment>();
        public UserProfile Sender { get; set; } = null!;
        public Guid SenderId { get; private set; }
        public void Create_Receiver(string firstName, string lastName, string email, string phone, string address, int cityId, Guid senderId)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phone;
            Address = address;
            UserType = UserType.Receiver;
            CreatedAt = DateTime.UtcNow;
            CityId = cityId;
            SenderId = senderId;
            CurrentState = true;
        }
        private ReceiverProfile()
        {

        }

        private ReceiverProfile(string firstName, string lastName, string email, string phone, string address, int cityId, Guid senderId)
        {
            Id  = Guid.CreateVersion7();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phone;
            Address = address;
            CreatedAt = DateTime.UtcNow;
            UserType = UserType.Receiver;
            CityId = cityId;
            SenderId = senderId;
            CurrentState = true;
        }

        public static ReceiverProfile Create(string firstName, string lastName, string email, string phone, string address, int cityId, Guid senderId)
        {
            return new ReceiverProfile(firstName, lastName, email, phone, address, cityId, senderId);

        }
    }
}   