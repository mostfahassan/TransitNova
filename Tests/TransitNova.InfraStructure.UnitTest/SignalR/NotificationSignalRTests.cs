using System.Reflection;
using FluentAssertions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Moq;
using TransitNova.BusinessLayer.DTOs.Notification;
using TransitNova.InfraStructure.SignalR;
using TransitNova.InfraStructure.SignalR.NotificationHubService;

namespace TransitNova.InfraStructure.Tests.SignalR;

public sealed class NotificationSignalRTests
{
    [Fact]
    public void NotificationHub_Should_RequireAuthenticatedUsersAsync()
    {
        typeof(NotificationHub)
            .GetCustomAttribute<AuthorizeAttribute>()
            .Should()
            .NotBeNull("notification streams must not be available to anonymous clients");
    }

    [Fact]
    public async Task SendToUserAsync_Should_TargetCurrentUserAndSendCompleteNotificationPayloadAsync()
    {
        var userId = Guid.NewGuid();
        var notification = new NotificationDto
        {
            Id = Guid.NewGuid(),
            Title = "Shipment updated",
            Message = "Your shipment status changed.",
            IsRead = false,
            CreatedOnUtc = DateTime.UtcNow
        };

        var proxy = new Mock<IClientProxy>();
        proxy.Setup(x => x.SendCoreAsync(
                "ReceiveNotification",
                It.IsAny<object?[]>(),
                It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var clients = new Mock<IHubClients>();
        clients.Setup(x => x.User(userId.ToString())).Returns(proxy.Object);

        var hubContext = new Mock<IHubContext<NotificationHub>>();
        hubContext.SetupGet(x => x.Clients).Returns(clients.Object);

        var broadcaster = new SignalRNotificationBroadcaster(hubContext.Object);

        await broadcaster.SendToUserAsync(userId, notification, CancellationToken.None);

        clients.Verify(x => x.User(userId.ToString()), Times.Once);
        proxy.Verify(x => x.SendCoreAsync(
            "ReceiveNotification",
            It.Is<object?[]>(args => args.Length == 1 && ReferenceEquals(args[0], notification)),
            CancellationToken.None), Times.Once);
    }
}