
using Microsoft.EntityFrameworkCore.Diagnostics;
using Newtonsoft.Json;
using TransitNova.Domain.Contracts.DomainEvents;
using TransitNova.InfraStructure.OutBox;
namespace TransitNova.InfraStructure.Common.Interceptors
{
    internal class ConvertDomainEventsToOutboxMessages : SaveChangesInterceptor
    {

        public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData,
            InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;
            if (context is null) return await base.SavingChangesAsync(eventData, result, cancellationToken);

            var outboxMessages = context.ChangeTracker
                .Entries<IAggregateRoot>()
                .Select(x => x.Entity)
                .SelectMany(aggregate =>
                {
                    var events = aggregate.GetDomainEvents().ToList();
                    aggregate.ClearDomainEvents();
                    return events;
                })
                .Select(domainEvent => new OutboxMessage
                {
                    Id = Guid.CreateVersion7(),
                    Type = domainEvent.GetType().AssemblyQualifiedName!,
                    OccuredAt = DateTime.UtcNow,
                    Content = JsonConvert.SerializeObject(domainEvent),
                    RetryCount = 0
                })
                .ToList();

            if (outboxMessages.Count > 0)
                context.Set<OutboxMessage>().AddRange(outboxMessages);
           

            return await base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}

