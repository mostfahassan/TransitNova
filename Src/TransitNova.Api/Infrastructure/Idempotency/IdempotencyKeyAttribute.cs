using Microsoft.AspNetCore.Mvc;

namespace TransitNova.Api.Infrastructure.Idempotency
{
    [AttributeUsage(AttributeTargets.Parameter)]
    public sealed class IdempotencyKeyAttribute : FromHeaderAttribute
    {
        public const string HeaderName = "X-Idempotency-Key";

        public IdempotencyKeyAttribute()
        {
            Name = HeaderName;
        }
    }
}
