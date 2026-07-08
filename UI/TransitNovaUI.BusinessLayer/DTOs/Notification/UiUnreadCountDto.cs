using ApiUnreadCountDto = TransitNova.BusinessLayer.DTOs.Notification.UnreadCountDto;

namespace TransitNovaUI.BusinessLayer.DTOs.Notification;

public sealed class UiUnreadCountDto
{
    public int Count { get; set; }

    public static UiUnreadCountDto ToDto(ApiUnreadCountDto source) => new()
    {
        Count = source.Count
    };
}