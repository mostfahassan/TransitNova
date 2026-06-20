using TransitNova.BusinessLayer.DTOs.ShipmentStatusDto;
using TransitNova.Domain.Enums.Shipment;
using TransitNovaUI.BusinessLayer.DTOs.UserProfile;

namespace TransitNovaUI.BusinessLayer.DTOs.ShipmentStatusDto;

public sealed class UiRetrieveShipmentStatusDto
{
    public ShipmentStatuses StatusType { get; set; }
    public DateTime ChangedAt { get; set; }
    public UiUserSummaryDto? Carrier { get; set; }

    public static UiRetrieveShipmentStatusDto ToUiDto(RetrieveShipmentStatusDto source) =>
        new()
        {
            StatusType = source.StatusType,
            ChangedAt = source.ChangedAt,
            Carrier = source.Carrier is null ? null : UiUserSummaryDto.ToUiDto(source.Carrier)
        };
}
