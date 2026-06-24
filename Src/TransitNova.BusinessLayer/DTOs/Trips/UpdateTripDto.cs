using TransitNova.Domain.Enums.Trip;

namespace TransitNova.BusinessLayer.DTOs.Trips
{
    public class UpdateTripDto
    {
        public Guid? CarrierId { get; set; }
        public Guid? WarehouseId { get; set; }
        public TripType? TripType { get; set; }
        public DateTime? PlannedDate { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? TotalShipments { get; set; }
    }
}
