using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using FluentAssertions;
using Microsoft.Extensions.Options;
using TransitNova.BusinessLayer.DTOs.AppUser;
using TransitNova.Domain.Contracts.Permissions;
using TransitNova.Domain.Contracts.Roles;
using TransitNova.Domain.Enums.Users;
using TransitNova.InfraStructure.Token;

namespace TransitNova.InfraStructure.Tests.Token;

public sealed class TokenGeneratorTests
{
    [Fact]
    public async Task GenerateTokenAsync_WarehouseManagerRole_Should_IncludeRoleAndAllWarehouseManagerPermissionsAsync()
    {
        var user = CreateUser(Role.WarehouseManager, UserType.WarehouseManager);
        var generator = CreateGenerator();

        var tokenText = await generator.GenerateTokenAsync(user);

        var token = ReadToken(tokenText);
        token.Claims.Should().Contain(c => c.Type == "nameid" && c.Value == user.Id.ToString());
        token.Claims.Should().Contain(c => c.Type == "email" && c.Value == user.Email);
        token.Claims.Should().Contain(c => c.Type == "role" && c.Value == Role.WarehouseManager);
        token.Claims.Should().Contain(c => c.Type == "user_Type" && c.Value == UserType.WarehouseManager.ToString());
        token.Claims.Where(c => c.Type == "Permission").Select(c => c.Value)
            .Should().BeEquivalentTo(WarehouseManagerPermissions.All);
    }

    [Fact]
    public async Task GenerateTokenAsync_MultipleRoles_Should_MergePermissionsWithoutDuplicatesAsync()
    {
        var user = CreateUser(Role.User, UserType.User);
        user.Roles.Add(Role.Carrier);
        var generator = CreateGenerator();

        var tokenText = await generator.GenerateTokenAsync(user);

        var token = ReadToken(tokenText);
        token.Claims.Where(c => c.Type == "role").Select(c => c.Value)
            .Should().BeEquivalentTo(Role.User, Role.Carrier);
        var permissions = token.Claims.Where(c => c.Type == "Permission").Select(c => c.Value).ToList();
        permissions.Should().Contain(UserPermissions.All);
        permissions.Should().Contain(CarrierPermissions.All);
        permissions.Should().OnlyHaveUniqueItems();
    }

    [Fact]
    public async Task GenerateTokenAsync_MissingJwtKey_Should_ThrowClearConfigurationExceptionAsync()
    {
        var generator = new TokenGenerator(Options.Create(new JwtSettings
        {
            Key = " ",
            Issuer = "TransitNova.Tests",
            Audience = "TransitNova.Tests"
        }));

        var action = () => generator.GenerateTokenAsync(CreateUser(Role.User, UserType.User));

        await action.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("JWT key is not configured.");
    }

    [Fact]
    public void GenerateRefreshToken_Should_ReturnRandomBase64TokenWithExpectedEntropyAsync()
    {
        var generator = CreateGenerator();

        var first = generator.GenerateRefreshToken();
        var second = generator.GenerateRefreshToken();

        first.Should().NotBe(second);
        Convert.FromBase64String(first).Should().HaveCount(32);
        Convert.FromBase64String(second).Should().HaveCount(32);
    }

    private static TokenGenerator CreateGenerator() => new(Options.Create(new JwtSettings
    {
        Key = "TransitNova integration and unit test signing key with enough bytes",
        Issuer = "TransitNova.Tests",
        Audience = "TransitNova.Tests"
    }));

    private static JwtSecurityToken ReadToken(string tokenText) =>
        new JwtSecurityTokenHandler().ReadJwtToken(tokenText);

    private static AppUserDto CreateUser(string role, UserType userType) => new()
    {
        Id = Guid.NewGuid(),
        Email = "user@example.com",
        UserName = "test-user",
        PhoneNumber = "+201000000000",
        UserType = userType,
        Roles = [role]
    };
}