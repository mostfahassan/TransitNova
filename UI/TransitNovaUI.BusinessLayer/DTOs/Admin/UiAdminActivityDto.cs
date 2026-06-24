using TransitNova.BusinessLayer.DTOs.Admin;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.Shipment;
namespace TransitNovaUI.BusinessLayer.DTOs.Admin;

public sealed class UiAdminActivityDto
{
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime OccurredAt { get; set; }
    public string PerformedBy { get; set; } = string.Empty;

    public static UiAdminActivityDto ToUiDto(AdminActivityDto source) =>
        new()
        {
            Title = source.Title,
            Description = source.Description,
            OccurredAt = source.OccurredAt,
            PerformedBy = source.PerformedBy
        };
}
