using TransitNova.Domain.Enums.Result;

public sealed record Error(ErrorCode Code, string Message);
