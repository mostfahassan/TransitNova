
namespace TransitNova.InfraStructure.OutBox
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; }
        public DateTime OccuredAt { get; set; }
        public DateTime? ProcessedOn { get; set; }
        public string? Content { get; set; }
        public string? Type {  get; set; }
        public string? Error { get; set; }
    }
}
