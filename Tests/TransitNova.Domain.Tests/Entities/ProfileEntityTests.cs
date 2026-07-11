using FluentAssertions;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Users;

namespace TransitNova.Domain.Tests.Entities;

public sealed class ProfileEntityTests
{
    [Fact]
    public void CreateUserProfile_Should_Create_UserProfile_When_DataIsValid()
    {
        var appUserId = Guid.NewGuid();

        var profile = UserProfile.Create(appUserId, "Omar", "Ali", "omar@example.com", "0100", Address.FromLegacy("Cairo"), 1);

        profile.Id.Should().NotBeEmpty();
        profile.AppUserId.Should().Be(appUserId);
        profile.FullName.Should().Be("Omar Ali");
        profile.UserType.Should().Be(UserType.User);
        profile.CurrentState.Should().BeTrue();
    }

    [Fact]
    public void CreateReceiverProfile_Should_LinkSender_When_DataIsValid()
    {
        var senderId = Guid.NewGuid();

        var profile = ReceiverProfile.Create("Mona", "Ali", "mona@example.com", "0101", Address.FromLegacy("Giza"), 2, senderId);

        profile.Id.Should().NotBeEmpty();
        profile.SenderId.Should().Be(senderId);
        profile.UserType.Should().Be(UserType.Receiver);
        profile.FullName.Should().Be("Mona Ali");
    }

    [Fact]
    public void CreateAdminProfile_Should_Create_AdminProfile_When_DataIsValid()
    {
        var appUserId = Guid.NewGuid();

        var profile = AdminProfile.Create(appUserId, "Aya", "Hassan", "aya@example.com", "0102", Address.FromLegacy("Cairo"), 1);

        profile.AppUserId.Should().Be(appUserId);
        profile.UserType.Should().Be(UserType.Admin);
        profile.FullName.Should().Be("Aya Hassan");
        profile.CurrentState.Should().BeTrue();
    }

    [Fact]
    public void CreateOperationManagerProfile_Should_Create_Manager_When_DataIsValid()
    {
        var appUserId = Guid.NewGuid();

        var profile = OperationManagerProfile.Create(appUserId, "Ali", "Samir", "ali@example.com", "0103", Address.FromLegacy("Cairo"), 1);

        profile.Id.Should().NotBeEmpty();
        profile.AppUserId.Should().Be(appUserId);
        profile.UserType.Should().Be(UserType.OperationManager);
        profile.HandledCarriers.Should().BeEmpty();
        profile.HandledShipments.Should().BeEmpty();
    }
}
