namespace TransitNova.UI.Infrastructure.Mvc;

public interface IIdempotencyKeyFactory
{
    string Create();
}
