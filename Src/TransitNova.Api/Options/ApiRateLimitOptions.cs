namespace TransitNova.Api.Options;

public sealed class ApiRateLimitOptions
{
    public const string SectionName = "RateLimiting";

    public int DefaultPermitLimit { get; init; } = 100;
    public int DefaultQueueLimit { get; init; } = 50;
    public int CommandPermitLimit { get; init; } = 20;
    public int CommandQueueLimit { get; init; } = 50;
    public int WindowSeconds { get; init; } = 60;
    public int SegmentsPerWindow { get; init; } = 6;
}
