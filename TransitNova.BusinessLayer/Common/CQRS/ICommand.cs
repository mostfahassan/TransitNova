using MediatR;

namespace TransitNova.BusinessLayer.Common.CQRS
{
    public interface ICommand : IRequest;
    public interface ICommand<TResponse> : IRequest<TResponse>;
}
