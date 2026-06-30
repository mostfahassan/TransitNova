using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;

namespace TransitNova.BusinessLayer.Common.Interfaces
{
    public abstract record IdempotentCommand<TResponse>(Guid RequestId) : ICommand<TResponse>, ITransactional;
}

