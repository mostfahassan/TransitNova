using FluentAssertions;
using TransitNova.Domain.Entities.Common;

namespace TransitNova.Domain.Tests.Common;

public sealed class PackageSpecificationTests
{
    [Fact]
    public void Equality_Should_ReturnTrue_When_All_Dimensions_AreEqual()
    {
        var first = new PackageSpecification(5m, 2m, 3m, 4m);
        var second = new PackageSpecification(5m, 2m, 3m, 4m);

        first.Should().Be(second);
        first.GetHashCode().Should().Be(second.GetHashCode());
    }

    [Fact]
    public void Equality_Should_ReturnFalse_When_Any_Dimension_Differs()
    {
        var first = new PackageSpecification(5m, 2m, 3m, 4m);
        var second = new PackageSpecification(6m, 2m, 3m, 4m);

        first.Should().NotBe(second);
    }
}
