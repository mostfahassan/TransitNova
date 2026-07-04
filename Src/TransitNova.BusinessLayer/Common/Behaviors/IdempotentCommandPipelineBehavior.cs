using MediatR;
using Microsoft.Extensions.Logging;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Interfaces.Repositories.IdempotentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.DomainExceptions;
namespace TransitNova.BusinessLayer.Common.Behaviors
{
    public sealed class IdempotentCommandPipelineBehavior<TRequest, TResponse>(
        IUnitOfWork unitOfWork,
        IIdempotentRepository idempotent,
        ILogger<IdempotentCommandPipelineBehavior<TRequest, TResponse>> logger)
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
            var idempotentResponse = await idempotent.ReturnRequestIfExistsAsync(request.RequestId, cancellationToken);

            if (idempotentResponse is not null)
            {
                if (idempotentResponse.hashedRequest != ComputeRequestHash(request))
                {
                    throw new ConflictRequestException("Idempotent request hash mismatch");
                }

                if (idempotentResponse.ResponseJson is not null)
                {
                    return JsonSerializer.Deserialize<TResponse>(idempotentResponse.ResponseJson, JsonOption)
                    ?? throw new InvalidOperationException("Corrupted idempotent cached response");
                }
            }
            try
            {
                var response = await next(cancellationToken);
                var serializeResponse = JsonSerializer.Serialize(response, JsonOption);
                var hashedRequest = ComputeRequestHash(request);
                await idempotent.CreateRequestAsync(request.RequestId, typeof(TRequest).Name, serializeResponse, hashedRequest, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
                return response;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to process idempotent request {RequestId}", request.RequestId);
                throw;
            }
        }
        private string ComputeRequestHash<THashedRequest>(THashedRequest request)
        {
            var payload = JsonSerializer.Serialize(request, request!.GetType(), JsonOption);

            using var sha256 = SHA256.Create();
            var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(payload));

            return Convert.ToBase64String(hashBytes);
        }
    }
}

