
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Admin
{

    public sealed class AdminActivityDto
    {
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        public DateTime OccurredAt { get; set; }

        public string PerformedBy { get; set; } = string.Empty;
    }

}
