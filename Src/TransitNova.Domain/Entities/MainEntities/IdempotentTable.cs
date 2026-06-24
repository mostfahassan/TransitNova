
namespace TransitNova.Domain.Entities.MainEntities
{
    public  class IdempotentTable
    {
        public Guid RequestId { get;init; }
        public string? InstanceName { get; init; }
        public string? ResponseJson { get;init; }
        public DateTime CreatedAt { get; init; }
    }
}
