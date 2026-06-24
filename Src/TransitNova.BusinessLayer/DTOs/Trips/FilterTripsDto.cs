using TransitNova.Domain.Enums.Trip;

namespace TransitNova.BusinessLayer.DTOs.Trips
{
    public class FilterTripsDto
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
    }
}
