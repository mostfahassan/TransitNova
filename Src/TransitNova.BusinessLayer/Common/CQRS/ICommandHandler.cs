using MediatR;

namespace TransitNova.BusinessLayer.Common.CQRS
{
    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>
    {
    }
}
