
namespace TransitNova.Domain.Contracts.DomainEvents.Events.NotificationEvents
{
    public sealed record NotificationCreatedDomainEvent(Guid Id , Guid UserId,string Title ,string Message) : IDomainEvent;
   
}
