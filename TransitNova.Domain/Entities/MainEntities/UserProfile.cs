using TransitNova.Domain.Contracts.DomainEvents;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class UserProfile : BaseInfo<Guid>
    {
        public virtual ICollection<Shipment> SentShipments { get; set; } = new List<Shipment>();
        public ICollection<BundleSubscription> Subscriptions { get; set; } = new List<BundleSubscription>();
        public Guid AppUserId { get; private set; } 
        public ICollection<ReceiverProfile> Receivers { get; set; } = new List<ReceiverProfile>();

        private UserProfile()
        {
            
        }

        private UserProfile(Guid Id, string firstName, string lastName, string email, string phone, string address, int cityId)
        {
            this.Id = Guid.CreateVersion7();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phone;
            Address = address;
            AppUserId = Id;
            UserType = UserType.User;
            CreatedAt = DateTime.UtcNow;
            CityId = cityId;
            CurrentState = true;
        }
        public static UserProfile Create(Guid Id, string firstName, string lastName, string email, string phone, string address, int cityId)
                    => new (Id, firstName, lastName, email, phone, address, cityId);
        
    }
}   
         
   