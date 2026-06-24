using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Admin;

public sealed class UiRevenueSummaryDto
{
    public decimal TotalRevenue { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal WeeklyRevenue { get; set; }
    public decimal DailyRevenue { get; set; }

    public static UiRevenueSummaryDto ToUiDto(RevenueSummaryDto source) =>
        new()
        {
            TotalRevenue = source.TotalRevenue,
            MonthlyRevenue = source.MonthlyRevenue,
            WeeklyRevenue = source.WeeklyRevenue,
            DailyRevenue = source.DailyRevenue
        };
}
