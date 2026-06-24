using MediatR;

namespace TransitNova.BusinessLayer.Common.CQRS
{
    public interface IQuery<TResponse> : IRequest<TResponse>;
}
