using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Interfaces.Repositories.IdempotentRepository;
namespace TransitNova.BusinessLayer.Common.Behaviors
{
    public sealed class IdempotentCommandPipelineBehavior<TRequest, TResponse>(
        IIdempotentRepository idempotent,ILogger<IdempotentCommandPipelineBehavior<TRequest, TResponse>> logger)
        : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IdempotentCommand<TResponse>
    {
        private readonly JsonSerializerOptions JsonOption = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        };
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var requestKeyResponse = await idempotent.ReturnRequestIfExistsAsync(request.RequestId, cancellationToken);

            if (requestKeyResponse is not null)
            {
                var idempotentResponse = JsonSerializer.Deserialize<TResponse>(requestKeyResponse, JsonOption) ?? throw new InvalidOperationException("Corrupted idempotent cached response");

                return idempotentResponse;
            }

            var response = await next(cancellationToken);

            var serializeResponse = JsonSerializer.Serialize(response, JsonOption);

            try
            {
                await idempotent.CreateRequestAsync(request.RequestId, typeof(TRequest).Name, serializeResponse, cancellationToken);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to store idempotent response for RequestId: {RequestId}", request.RequestId);

            }
            return response;
        }
    }
}