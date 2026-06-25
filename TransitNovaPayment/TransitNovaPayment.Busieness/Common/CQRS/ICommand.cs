

using MediatR;
namespace TransitNovaPayment.Busieness.Common.CQRS
{
    public interface ICommand<TResponse> : IRequest<TResponse>;

    public interface ICommandHandler<TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
        where TCommand : ICommand<TResponse>;
}
