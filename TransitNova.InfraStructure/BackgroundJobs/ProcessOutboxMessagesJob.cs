using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using TransitNova.Domain.Contracts.DomainEvents;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.BackgroundJobs
{
    [DisallowConcurrentExecution]
    public sealed class ProcessOutboxMessagesJob(AppDbContext dbContext, IPublisher publisher) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            var messages = await dbContext.OutboxMessages
                .Where(x => x.ProcessedOn == null && x.RetryCount < 5)
                .OrderBy(x => x.OccuredAt)
                .Take(20)
                .ToListAsync(context.CancellationToken);

            foreach (var message in messages)
            {
                try
                {
                    var type = Type.GetType(message.Type ?? string.Empty)
                        ?? throw new InvalidOperationException($"Unknown type: {message.Type}");

                    var domainEvent = (IDomainEvent?)JsonConvert.DeserializeObject(message.Content ?? string.Empty, type)
                        ?? throw new InvalidOperationException("Deserialization returned null");

                    await publisher.Publish(domainEvent, context.CancellationToken);

                    message.ProcessedOn = DateTime.UtcNow;
                }
                catch (Exception ex)
                {
                    message.RetryCount++;
                    message.Error = ex.Message;
                }
                await dbContext.SaveChangesAsync(context.CancellationToken);
            }
        }
    }
}
