using ApiNotificationDto = TransitNova.BusinessLayer.DTOs.Notification.NotificationDto;

namespace TransitNovaUI.BusinessLayer.DTOs.Notification;

public sealed class UiNotificationDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime CreatedOnUtc { get; set; }

    public static UiNotificationDto ToDto(ApiNotificationDto source) => new()
    {
        Id = source.Id,
        Title = source.Title,
        Message = source.Message,
        IsRead = source.IsRead,
        CreatedOnUtc = source.CreatedOnUtc
    };
}