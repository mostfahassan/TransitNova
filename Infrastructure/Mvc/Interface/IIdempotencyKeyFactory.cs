namespace TransitNova.UI.Infrastructure.Mvc.Interface;

public interface IIdempotencyKeyFactory
{
    string Create();
}
