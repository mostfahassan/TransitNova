using MediatR;

namespace TransitNova.BusinessLayer.Common.CQRS
{
    public interface IQueryHandler<TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
        where TQuery : IQuery<TResponse>
    {
    }
}
