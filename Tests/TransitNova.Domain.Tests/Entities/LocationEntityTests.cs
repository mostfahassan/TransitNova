using FluentAssertions;
using TransitNova.Domain.Entities.MainEntities;

namespace TransitNova.Domain.Tests.Entities;

public sealed class LocationEntityTests
{
    [Fact]
    public void CreateCountry_Should_Create_ActiveCountry_When_DataIsValid()
    {
        var before = DateTime.UtcNow;

        var country = Country.Create("Egypt");

        country.Name.Should().Be("Egypt");
        country.CurrentState.Should().BeTrue();
        country.CreatedAt.Should().BeOnOrAfter(before);
    }

    [Fact]
    public void UpdateCountry_Should_Update_NameAndTimestamp_When_Called()
    {
        var country = Country.Create("Egypt");

        country.Update("Arab Republic of Egypt");

        country.Name.Should().Be("Arab Republic of Egypt");
        country.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void CreateGovernment_Should_SetCountryAndName_When_DataIsValid()
    {
        var government = Government.Create("Cairo", 7);

        government.Name.Should().Be("Cairo");
        government.CountryId.Should().Be(7);
        government.CurrentState.Should().BeTrue();
    }

    [Fact]
    public void UpdateGovernment_Should_UpdateCountryAndName_When_Called()
    {
        var government = Government.Create("Cairo", 7);

        government.Update("Giza", 8);

        government.Name.Should().Be("Giza");
        government.CountryId.Should().Be(8);
        government.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void CreateCity_Should_SetGovernmentAndName_When_DataIsValid()
    {
        var city = City.Create("Nasr City", 3);

        city.Name.Should().Be("Nasr City");
        city.GovernmentId.Should().Be(3);
        city.CurrentState.Should().BeTrue();
        city.Zones.Should().BeEmpty();
    }

    [Fact]
    public void UpdateCity_Should_UpdateGovernmentAndName_When_Called()
    {
        var city = City.Create("Nasr City", 3);

        city.Update("Heliopolis", 4);

        city.Name.Should().Be("Heliopolis");
        city.GovernmentId.Should().Be(4);
        city.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void CreateZone_Should_Create_ActiveZone_When_DataIsValid()
    {
        var zone = Zone.Create("Downtown", 2);

        zone.Name.Should().Be("Downtown");
        zone.Code.Should().StartWith("DOW-");
        zone.Code.Should().HaveLength(8);
        zone.CityId.Should().Be(2);
        zone.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void UpdateZone_Should_TrimValuesAndUpdateTimestamp_When_Called()
    {
        var zone = Zone.Create("Downtown", 2);

        zone.Update(" New Zone ", " NZ-2 ", 5);

        zone.Name.Should().Be("New Zone");
        zone.Code.Should().Be("NZ-2");
        zone.CityId.Should().Be(5);
        zone.UpdatedAt.Should().NotBeNull();
    }
}
