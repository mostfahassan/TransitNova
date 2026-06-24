
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.DTOs.Admin
{

    public sealed class RevenueSummaryDto
    {
        public decimal TotalRevenue { get; set; }

        public decimal MonthlyRevenue { get; set; }

        public decimal WeeklyRevenue { get; set; }

        public decimal DailyRevenue { get; set; }
    }

}
