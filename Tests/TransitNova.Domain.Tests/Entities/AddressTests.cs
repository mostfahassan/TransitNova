using FluentAssertions;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;

namespace TransitNova.Domain.Tests.Entities;

public sealed class AddressTests
{
    [Fact]
    public void Create_Should_TrimAndComposeAddress_WhenValuesAreValid()
    {
        var address = Address.Create(" Building 12 ", " Floor 3, Apt 8 ", " North 90 Street ");

        address.MainAddress.Should().Be("Building 12");
        address.SecondaryAddress.Should().Be("Floor 3, Apt 8");
        address.Street.Should().Be("North 90 Street");
        address.ToString().Should().Be("Building 12, Floor 3, Apt 8, North 90 Street");
    }

    [Theory]
    [InlineData("", "Street")]
    [InlineData("Building", "")]
    public void Create_Should_RejectMissingRequiredValues(string mainAddress, string street)
    {
        var act = () => Address.Create(mainAddress, null, street);

        act.Should().Throw<DomainOperationException>()
            .Which.ErrorCode.Should().Be("INVALID_ADDRESS");
    }

    [Fact]
    public void FromLegacy_Should_PreserveLegacyText()
    {
        var address = Address.FromLegacy("Legacy Cairo address");

        address.MainAddress.Should().Be("Legacy Cairo address");
        address.SecondaryAddress.Should().BeNull();
        address.Street.Should().Be("Unknown");
    }
}
