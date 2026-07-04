using TransitNova.BusinessLayer.DTOs.Trips;
using TransitNova.Domain.Enums.Trip;

namespace TransitNovaUI.BusinessLayer.DTOs.Trips;

public sealed class UiFilterTripsDto
{
    public TripType? TripType { get; set; }
    public TripStatus[]? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? CreatedBy { get; set; }
    public Guid? CarrierId { get; set; }
    public Guid? WarehouseId { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    public static FilterTripsDto ToDto(UiFilterTripsDto source) =>
        new()
        {
            TripType = source.TripType,
            Status = source.Status is null ? null : [.. source.Status],
            CreatedAt = source.CreatedAt,
            From = source.From,
            To = source.To,
            CreatedBy = source.CreatedBy,
            CarrierId = source.CarrierId,
            WarehouseId = source.WarehouseId,
            PageNumber = source.PageNumber,
            PageSize = source.PageSize
        };
}
