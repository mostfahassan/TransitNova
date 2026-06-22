using MediatR;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.Repositories.IdempotentRepository;
using static TransitNova.BusinessLayer.Common.Exceptions.ReusedRefreshTokenException;

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
                throw new IdempotentConflicExceptionException();

            await idempotent.CreateRequestAsync(request.RequestId, typeof(TRequest).Name, cancellationToken);

            var response = await next(cancellationToken);

            return response;
        }
    }
}
