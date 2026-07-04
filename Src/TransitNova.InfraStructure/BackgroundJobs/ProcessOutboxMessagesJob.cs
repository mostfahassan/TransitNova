using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Quartz;
using TransitNova.Domain.Contracts.DomainEvents;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.BackgroundJobs;

[DisallowConcurrentExecution]
public sealed class ProcessOutboxMessagesJob(AppDbContext dbContext, IPublisher publisher, ILogger<ProcessOutboxMessagesJob> logger) : IJob

{
    private const int MaxRetryCount = 5;
    private const int BatchSize = 20;

    public async Task Execute(IJobExecutionContext context)
    {
        var messages = await dbContext.OutboxMessages
            .Where(m => m.ProcessedOn == null && m.RetryCount < MaxRetryCount)
            .OrderBy(m => m.OccuredAt)
            .Take(BatchSize)
            .ToListAsync(context.CancellationToken);

        if (messages.Count == 0)
        {
            logger.LogDebug("No outbox messages found for processing.");
            return;
        }
        logger.LogInformation("Processing {Count} outbox messages.", messages.Count);
        foreach (var message in messages)
        {
            try
            {
                var eventType = Type.GetType(message.Type ?? string.Empty) ?? throw new InvalidOperationException($"Unable to resolve event type '{message.Type}'.");

                var domainEvent = JsonConvert.DeserializeObject(message.Content ?? string.Empty, eventType) as IDomainEvent ?? throw new InvalidOperationException($"Failed to deserialize outbox message '{message.Id}'.");


                await publisher.Publish(domainEvent, context.CancellationToken);

                message.ProcessedOn = DateTime.UtcNow;
                message.Error = null;

                logger.LogInformation("Successfully processed outbox message {MessageId}.", message.Id);
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.ToString();

                logger.LogError(ex, "Failed to process outbox message {MessageId}. Retry Count: {RetryCount}", message.Id, message.RetryCount);
            }

        }

        await dbContext.SaveChangesAsync(context.CancellationToken);
        logger.LogInformation("Finished processing outbox messages batch.");
    }
}
