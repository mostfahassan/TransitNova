using MediatR;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Interfaces.Repositories.IdempotentRepository;
namespace TransitNova.BusinessLayer.Common.Behaviors
{
    public sealed class IdempotentCommandPipelineBehavior<TRequest, TResponse>(
        IIdempotentRepository idempotent)
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

            var SerializedResponse = JsonSerializer.Serialize(response, JsonOption);

            await idempotent.CreateRequestAsync(request.RequestId, typeof(TRequest).Name, SerializedResponse, cancellationToken);

            return response;
        }
    }
}