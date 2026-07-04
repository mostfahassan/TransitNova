namespace TransitNova.UI.Infrastructure.Mvc;

public sealed class IdempotencyKeyFactory : IIdempotencyKeyFactory
{
    public string Create() => Guid.CreateVersion7().ToString();
}
