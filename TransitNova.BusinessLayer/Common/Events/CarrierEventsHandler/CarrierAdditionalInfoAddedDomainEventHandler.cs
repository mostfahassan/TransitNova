using MediatR;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
namespace TransitNova.BusinessLayer.Common.Events.CarrierEventsHandler
{
    public class CarrierAdditionalInfoAddedDomainEventHandler
        : INotificationHandler<CarrierAdditionalInfoAddedDomainEvent>
    {
        public Task Handle(CarrierAdditionalInfoAddedDomainEvent notification, CancellationToken cancellationToken)
            => Task.CompletedTask;
    }
}