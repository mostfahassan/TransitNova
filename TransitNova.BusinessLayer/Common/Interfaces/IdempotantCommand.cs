
using TransitNova.BusinessLayer.Common.CQRS;
namespace TransitNova.BusinessLayer.Common.Interfaces
{
    public abstract record IdempotantCommand<TResponse>(Guid RequestId) : ICommand<TResponse>;
}
