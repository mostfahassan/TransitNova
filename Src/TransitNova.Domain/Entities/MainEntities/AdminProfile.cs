using TransitNova.Domain.Contracts.DomainEvents;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class AdminProfile : BaseInfo<Guid>
    {
        public Guid AppUserId { get; private set; }
        private AdminProfile()
        {
            

        }


        private AdminProfile(Guid appUserId, string firstName, string lastName,string email,string phone, string address, int cityId)
        {
            Id = Guid.CreateVersion7();
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phone;
            Address = address;
            AppUserId = appUserId;
            UserType = UserType.Admin;
         
            CurrentState = true;
            CityId = cityId;
        }
        public static AdminProfile Create(Guid Id, string firstName, string lastName, string email, string phone, string address, int cityId)
        {
            return new (Id, firstName, lastName, email, phone, address, cityId);
        }
    }
}   
        