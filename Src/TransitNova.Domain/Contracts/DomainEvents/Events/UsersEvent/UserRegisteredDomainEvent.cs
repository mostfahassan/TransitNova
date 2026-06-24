
using TransitNova.Domain.Enums.Users;
namespace TransitNova.Domain.Contracts.DomainEvents.Events.UsersEvent
{
    public sealed record UserRegisteredDomainEvent(Guid Id , string Email , string FullName,string PhoneNumber , UserType Role) : IDomainEvent;
   
}
