using MediatR;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Interfaces.Repositories.Idempotent;

namespace TransitNova.BusinessLayer.Common.Behaviors
{
    public sealed class IdempotentCommandPipelineBehaviour<TRequest, TResponse>(IIdempotentRepository idempotent) :
        IPipelineBehavior<TRequest, TResponse>
        where TRequest : IdempotantCommand<TResponse>
    {
        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
        {
            var RequestKeyExists = await idempotent.RequestExistsAsync(request.RequestId, cancellationToken);
            if (RequestKeyExists)
                return default!;
            await idempotent.CreateRequestAsync(request.RequestId, typeof(TRequest).Name, cancellationToken);

            var response = await next();

            return response;
        }
    }
}
