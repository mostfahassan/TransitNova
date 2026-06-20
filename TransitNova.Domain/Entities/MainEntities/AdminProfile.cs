using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class AdminProfile : BaseInfo
    {
        public Guid AppUserId { get; private set; }
        private AdminProfile()
        {
            

        }


        private AdminProfile(Guid Id, string firstName, string lastName,string email,string phone, string address, int cityId)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phone;
            Address = address;
            AppUserId = Id;
            UserType = UserType.OperationManager;
            CreatedAt = DateTime.UtcNow;
            CurrentState = true;
            CityId = cityId;
        }
        public static AdminProfile Create(Guid Id, string firstName, string lastName, string email, string phone, string address, int cityId)
        {
            return new (Id, firstName, lastName, email, phone, address, cityId);
        }
    }
}   
        