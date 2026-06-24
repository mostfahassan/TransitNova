
using TransitNova.BusinessLayer.Common.CQRS;
namespace TransitNova.BusinessLayer.Common.Interfaces
{
    public abstract record IdempotentCommand<TResponse>(Guid RequestId) : ICommand<TResponse>;
}
