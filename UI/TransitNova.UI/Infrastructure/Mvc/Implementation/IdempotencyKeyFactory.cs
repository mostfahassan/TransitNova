using TransitNova.UI.Infrastructure.Mvc.Interface;

namespace TransitNova.UI.Infrastructure.Mvc.Implementation;

public sealed class IdempotencyKeyFactory : IIdempotencyKeyFactory
{
    public string Create() => Guid.CreateVersion7().ToString();
}
