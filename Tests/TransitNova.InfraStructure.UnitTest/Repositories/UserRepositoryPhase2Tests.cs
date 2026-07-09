using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.InfraStructure.Repository.User;
using TransitNova.InfraStructure.Tests.TestInfrastructure;

namespace TransitNova.InfraStructure.Tests.Repositories;

public sealed class UserRepositoryPhase2Tests
{
    [Fact]
    public async Task GetAppUserId_ShouldReturnProfileId_WhenProfileExistsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var user = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City);
        var result = await CreateRepository(fixture).GetAppUserIdAsync(user.AppUser.Id, default);
        result.Should().Be(user.Profile.Id);
    }

    [Fact]
    public async Task GetAppUserId_ShouldReturnEmptyGuid_WhenProfileDoesNotExistAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await CreateRepository(fixture).GetAppUserIdAsync(Guid.NewGuid(), default)).Should().BeEmpty();
    }

    [Fact]
    public async Task GetUserProfileAsync_ShouldProjectLocationAndIdentityFields_WhenProfileExistsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var user = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City);
        var result = await CreateRepository(fixture).GetUserProfileAsync(user.AppUser.Id, default);
        result.Should().NotBeNull();
        result!.FullName.Should().Be("Amina One");
        result.CityName.Should().Be(location.City.Name);
        result.CountryName.Should().Be(location.Country.Name);
    }

    [Fact]
    public async Task GetUserProfileAsync_ShouldReturnNull_WhenProfileDoesNotExistAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await CreateRepository(fixture).GetUserProfileAsync(Guid.NewGuid(), default)).Should().BeNull();
    }

    [Fact]
    public async Task GetUsersList_ShouldReturnEveryProfile_WhenMultipleUsersExistAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        await Phase2RepositoryTestData.AddUserAsync(fixture, location.City, "One");
        await Phase2RepositoryTestData.AddUserAsync(fixture, location.City, "Two");
        (await CreateRepository(fixture).GetUsersAsync(default)).Should().HaveCount(2);
    }

    [Theory]
    [InlineData("Amina", null, null, null)]
    [InlineData(null, "user-One@example.com", null, null)]
    [InlineData(null, null, "user-One", null)]
    [InlineData(null, null, null, "01000000000")]
    public async Task FilterUsersAsync_ShouldApplyTextCriterion_WhenCriterionMatchesAsync(
        string? search, string? email, string? userName, string? phone)
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        await Phase2RepositoryTestData.AddUserAsync(fixture, location.City, "One");
        var filter = new UserFiltrationDto
        {
            SearchTerm = search, Email = email, UserName = userName, PhoneNumber = phone
        };
        var result = await CreateRepository(fixture).FilterUsersAsync(filter, default);
        result.Data.Should().ContainSingle();
        result.TotalCount.Should().Be(1);
    }

    [Fact]
    public async Task FilterUsersAsync_ShouldUseDefaultPagination_WhenValuesAreNonPositiveAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var result = await CreateRepository(fixture)
            .FilterUsersAsync(new UserFiltrationDto { PageNumber = 0, PageSize = 0 }, default);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(20);
    }

    [Fact]
    public async Task GetUserDetailsForAdminAsync_ShouldReturnLockoutAndProfileData_WhenUserExistsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var user = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City);
        user.AppUser.LockoutEnd = DateTimeOffset.UtcNow.AddHours(1);
        await fixture.Context.SaveChangesAsync();
        var result = await CreateRepository(fixture).GetUserDetailsForAdminAsync(user.AppUser.Id, default);
        result!.IsLockedOut.Should().BeTrue();
        result.ProfileId.Should().Be(user.Profile.Id);
    }

    [Fact]
    public async Task GetShipmentCountInStatusAsync_ShouldReturnEmptyDictionary_WhenUserHasNoShipmentsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await CreateRepository(fixture).GetShipmentCountInStatusAsync(Guid.NewGuid(), default)).Should().BeEmpty();
    }

    [Fact]
    public async Task GetShipmentCountInStatusAsync_ShouldGroupOwnedShipments_WhenShipmentExistsAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var user = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City);
        await Phase2RepositoryTestData.AddShipmentAsync(fixture, user.Profile, location.City);
        var result = await CreateRepository(fixture).GetShipmentCountInStatusAsync(user.AppUser.Id, default);
        result.Should().ContainKey(ShipmentStatuses.Pending).WhoseValue.Should().Be(1);
    }

    [Fact]
    public async Task OwnsShipmentAsync_ShouldReturnTrue_WhenShipmentBelongsToUserAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        var location = await Phase2RepositoryTestData.AddLocationAsync(fixture);
        var user = await Phase2RepositoryTestData.AddUserAsync(fixture, location.City);
        var shipment = await Phase2RepositoryTestData.AddShipmentAsync(fixture, user.Profile, location.City);
        (await new UserRulesRepository(fixture.Context)
            .OwnsShipmentAsync(user.AppUser.Id, shipment.Id, default)).Should().BeTrue();
    }

    [Fact]
    public async Task OwnsShipmentAsync_ShouldReturnFalse_WhenShipmentDoesNotBelongToUserAsync()
    {
        await using var fixture = await SqliteAppDbContextFixture.CreateAsync();
        (await new UserRulesRepository(fixture.Context)
            .OwnsShipmentAsync(Guid.NewGuid(), Guid.NewGuid(), default)).Should().BeFalse();
    }

    private static UserQueryRepository CreateRepository(SqliteAppDbContextFixture fixture) =>
        new(fixture.Context, Phase2RepositoryTestData.CreateMapper());
}

