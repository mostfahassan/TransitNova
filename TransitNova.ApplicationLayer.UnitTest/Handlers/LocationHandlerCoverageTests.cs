using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Country;
using TransitNova.BusinessLayer.DTOs.ZoneDtos;
using TransitNova.BusinessLayer.Features.Location.Countries.Commands;
using TransitNova.BusinessLayer.Features.Location.Countries.Handlers.ApplyCommands;
using TransitNova.BusinessLayer.Features.Location.Countries.Handlers.ApplyingQueries;
using TransitNova.BusinessLayer.Features.Location.Countries.Queries;
using TransitNova.BusinessLayer.Features.Location.Governments.Commands;
using TransitNova.BusinessLayer.Features.Location.Governments.Handlers.ApplyCommands;
using TransitNova.BusinessLayer.Features.Location.Governments.Handlers.ApplyingQueries;
using TransitNova.BusinessLayer.Features.Location.Governments.Queries;
using TransitNova.BusinessLayer.Features.Zones.Commands;
using TransitNova.BusinessLayer.Features.Zones.Handlers.ApplyCommands;
using TransitNova.BusinessLayer.Features.Zones.Handlers.ApplyQueries;
using TransitNova.BusinessLayer.Features.Zones.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.ApplicationLayer.Tests.Handlers;

public sealed class LocationHandlerCoverageTests
{
    [Fact]
    public async Task CreateCountryHandler_Should_TrimPersistAndReturnCountry_When_RequestIsValid()
    {
        var repository = new Mock<ICountryRepository>();
        var unitOfWork = SuccessfulUnitOfWork();
        Country? captured = null;
        repository.Setup(x => x.AddAsync(It.IsAny<Country>(), It.IsAny<CancellationToken>()))
            .Callback<Country, CancellationToken>((country, _) => captured = country)
            .Returns(Task.CompletedTask);
        var handler = new CreateCountryHandler(repository.Object, unitOfWork.Object);

        var result = await handler.Handle(
            new CreateCountryCommand(Guid.NewGuid(), new CreateCountryDto { Name = "  Egypt  " }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Egypt");
        captured!.Name.Should().Be("Egypt");
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateCountryHandler_Should_PropagateExceptionAndSkipCommit_When_RepositoryFails()
    {
        var repository = new Mock<ICountryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        repository.Setup(x => x.AddAsync(It.IsAny<Country>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("database unavailable"));
        var handler = new CreateCountryHandler(repository.Object, unitOfWork.Object);

        var act = () => handler.Handle(
            new CreateCountryCommand(Guid.NewGuid(), new CreateCountryDto { Name = "Egypt" }),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("database unavailable");
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCountryHandler_Should_ThrowNotFoundAndSkipWrites_When_CountryDoesNotExist()
    {
        var repository = new Mock<ICountryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        repository.Setup(x => x.GetByIdAsync<Country>(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync((Country?)null);
        var handler = new UpdateCountryHandler(repository.Object, unitOfWork.Object, Mock.Of<ILogger<UpdateCountryHandler>>());

        var act = () => handler.Handle(
            new UpdateCountryCommand(Guid.NewGuid(), new UpdateCountryDto { CountryId = 42, Name = "Egypt" }),
            CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>().Where(x => x.ErrorCode == "COUNTRY_NOT_FOUNDED");
        repository.Verify(x => x.Update(It.IsAny<Country>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateCountryHandler_Should_UpdateAndCommit_When_CountryExists()
    {
        var country = Country.Create("Old");
        var repository = new Mock<ICountryRepository>();
        var unitOfWork = SuccessfulUnitOfWork();
        repository.Setup(x => x.GetByIdAsync<Country>(7, It.IsAny<CancellationToken>())).ReturnsAsync(country);
        var handler = new UpdateCountryHandler(repository.Object, unitOfWork.Object, Mock.Of<ILogger<UpdateCountryHandler>>());

        var result = await handler.Handle(
            new UpdateCountryCommand(Guid.NewGuid(), new UpdateCountryDto { CountryId = 7, Name = "  Egypt  " }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        country.Name.Should().Be("Egypt");
        repository.Verify(x => x.Update(country), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteCountryHandler_Should_DeleteAndCommit_When_CommandIsHandled()
    {
        var repository = new Mock<ICountryRepository>();
        var unitOfWork = SuccessfulUnitOfWork();
        repository.Setup(x => x.GetByIdAsync<Country>(7, It.IsAny<CancellationToken>())).ReturnsAsync(Country.Create("Egypt"));
        repository.Setup(x => x.DeleteAsync(7, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var handler = new DeleteCountryHandler(repository.Object, unitOfWork.Object, Mock.Of<ILogger<DeleteCountryHandler>>());

        var result = await handler.Handle(new DeleteCountryCommand(Guid.NewGuid(), 7), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        repository.Verify(x => x.DeleteAsync(7, CancellationToken.None), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task GetCountryByIdHandler_Should_ReturnNotFound_When_RepositoryReturnsNull()
    {
        var repository = new Mock<ICountryRepository>();
        repository.Setup(x => x.GetByIdAsync<CountryDto>(8, It.IsAny<CancellationToken>())).ReturnsAsync((CountryDto?)null);
        var handler = new GetCountryByIdHandler(repository.Object);

        var result = await handler.Handle(new GetCountryByIdQuery(8), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        result.Error!.Message.Should().Be("Country not found.");
    }

    [Fact]
    public async Task GetCountriesHandler_Should_ReturnEmptyCollection_When_NoCountriesExist()
    {
        var repository = new Mock<ICountryRepository>();
        repository.Setup(x => x.GetListAsync<CountryDto>(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var handler = new GetCountriesHandler(repository.Object);

        var result = await handler.Handle(new GetCountriesQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateGovernmentHandler_Should_ReturnNotFoundAndSkipWrites_When_CountryDoesNotExist()
    {
        var governments = new Mock<IGenericRepository<Government, int>>();
        var countries = new Mock<IGenericRepository<Country, int>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        countries.Setup(x => x.ExistsAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var handler = new CreateGovernmentHandler(governments.Object, countries.Object, unitOfWork.Object, Mock.Of<ILogger<CreateGovernmentHandler>>());

        var result = await handler.Handle(new CreateGovernmentCommand(Guid.NewGuid(), "Cairo", 99), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        governments.Verify(x => x.AddAsync(It.IsAny<Government>(), It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CreateGovernmentHandler_Should_TrimPersistAndReturnGovernment_When_CountryExists()
    {
        var governments = new Mock<IGenericRepository<Government, int>>();
        var countries = new Mock<IGenericRepository<Country, int>>();
        var unitOfWork = SuccessfulUnitOfWork();
        countries.Setup(x => x.ExistsAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        Government? captured = null;
        governments.Setup(x => x.AddAsync(It.IsAny<Government>(), It.IsAny<CancellationToken>()))
            .Callback<Government, CancellationToken>((government, _) => captured = government)
            .Returns(Task.CompletedTask);
        var handler = new CreateGovernmentHandler(governments.Object, countries.Object, unitOfWork.Object, Mock.Of<ILogger<CreateGovernmentHandler>>());

        var result = await handler.Handle(new CreateGovernmentCommand(Guid.NewGuid(), "  Cairo  ", 1), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Cairo");
        captured!.CountryId.Should().Be(1);
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task UpdateGovernmentHandler_Should_ReturnNotFound_When_GovernmentDoesNotExist()
    {
        var governments = new Mock<IGenericRepository<Government, int>>();
        var countries = new Mock<IGenericRepository<Country, int>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        governments.Setup(x => x.GetByIdAsync<Government>(4, It.IsAny<CancellationToken>())).ReturnsAsync((Government?)null);
        var handler = new UpdateGovernmentHandler(governments.Object, countries.Object, unitOfWork.Object, Mock.Of<ILogger<UpdateGovernmentHandler>>());

        var result = await handler.Handle(new UpdateGovernmentCommand(Guid.NewGuid(), 4, "Cairo", 1), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        countries.Verify(x => x.ExistsAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateGovernmentHandler_Should_ReturnNotFoundAndSkipUpdate_When_CountryDoesNotExist()
    {
        var government = Government.Create("Old", 1);
        var governments = new Mock<IGenericRepository<Government, int>>();
        var countries = new Mock<IGenericRepository<Country, int>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        governments.Setup(x => x.GetByIdAsync<Government>(4, It.IsAny<CancellationToken>())).ReturnsAsync(government);
        countries.Setup(x => x.ExistsAsync(99, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var handler = new UpdateGovernmentHandler(governments.Object, countries.Object, unitOfWork.Object, Mock.Of<ILogger<UpdateGovernmentHandler>>());

        var result = await handler.Handle(new UpdateGovernmentCommand(Guid.NewGuid(), 4, "Cairo", 99), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        governments.Verify(x => x.Update(It.IsAny<Government>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateGovernmentHandler_Should_UpdateAndCommit_When_EntitiesExist()
    {
        var government = Government.Create("Old", 1);
        var governments = new Mock<IGenericRepository<Government, int>>();
        var countries = new Mock<IGenericRepository<Country, int>>();
        var unitOfWork = SuccessfulUnitOfWork();
        governments.Setup(x => x.GetByIdAsync<Government>(4, It.IsAny<CancellationToken>())).ReturnsAsync(government);
        countries.Setup(x => x.ExistsAsync(2, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var handler = new UpdateGovernmentHandler(governments.Object, countries.Object, unitOfWork.Object, Mock.Of<ILogger<UpdateGovernmentHandler>>());

        var result = await handler.Handle(new UpdateGovernmentCommand(Guid.NewGuid(), 4, "  Giza  ", 2), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        government.Name.Should().Be("Giza");
        government.CountryId.Should().Be(2);
        governments.Verify(x => x.Update(government), Times.Once);
    }

    [Theory]
    [InlineData(false, ResultStatus.NotFound)]
    [InlineData(true, ResultStatus.Success)]
    public async Task DeleteGovernmentHandler_Should_ReturnExpectedResult_When_DeleteOutcomeIsKnown(bool deleted, ResultStatus expectedStatus)
    {
        var repository = new Mock<IGenericRepository<Government, int>>();
        var unitOfWork = SuccessfulUnitOfWork();
        repository.Setup(x => x.GetByIdAsync<Government>(5, It.IsAny<CancellationToken>())).ReturnsAsync(Government.Create("Cairo", 1));
        repository.Setup(x => x.DeleteAsync(5, It.IsAny<CancellationToken>())).ReturnsAsync(deleted);
        var handler = new DeleteGovernmentHandler(repository.Object, unitOfWork.Object, Mock.Of<ILogger<DeleteGovernmentHandler>>());

        var result = await handler.Handle(new DeleteGovernmentCommand(Guid.NewGuid(), 5), CancellationToken.None);

        result.Status.Should().Be(expectedStatus);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), deleted ? Times.Once() : Times.Never());
    }

    [Fact]
    public async Task GetGovernmentByIdHandler_Should_ReturnGovernment_When_EntityExists()
    {
        var repository = new Mock<IGenericRepository<Government, int>>();
        var dto = new GovernmentDto { Id = 4, Name = "Cairo" };
        repository.Setup(x => x.GetByIdAsync<GovernmentDto>(4, It.IsAny<CancellationToken>())).ReturnsAsync(dto);
        var handler = new GetGovernmentByIdHandler(repository.Object);

        var result = await handler.Handle(new GetGovernmentByIdQuery(4), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeSameAs(dto);
    }

    [Fact]
    public async Task GetGovernmentsHandler_Should_ReturnEmptyList_When_RepositoryIsEmpty()
    {
        var repository = new Mock<IGenericRepository<Government, int>>();
        repository.Setup(x => x.GetListAsync<GovernmentDto>(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var handler = new GetGovernmentsHandler(repository.Object);

        var result = await handler.Handle(new GetGovernmentsQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateZoneHandler_Should_ReturnCreatedZone_When_PersistedZoneCanBeRetrieved()
    {
        var repository = new Mock<IZoneRepository>();
        var unitOfWork = SuccessfulUnitOfWork();
        repository.Setup(x => x.GetByIdAsync<ZoneDto>(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Guid id, CancellationToken _) => new ZoneDto { Id = id, Name = "Downtown", Code = "DT", CityId = 1 });
        var handler = new CreateZoneHandler(repository.Object, unitOfWork.Object);

        var result = await handler.Handle(
            new CreateZoneCommand(Guid.NewGuid(), new CreateZoneDto { Name = " Downtown ", Code = " DT ", CityId = 1 }),
            CancellationToken.None);

        result.Status.Should().Be(ResultStatus.Created);
        result.Data!.Name.Should().Be("Downtown");
        repository.Verify(x => x.AddAsync(It.Is<Zone>(z => z.Name == "Downtown" && z.Code == "DT"), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CreateZoneHandler_Should_ReturnFailure_When_PersistedZoneCannotBeRetrieved()
    {
        var repository = new Mock<IZoneRepository>();
        var unitOfWork = SuccessfulUnitOfWork();
        repository.Setup(x => x.GetByIdAsync<ZoneDto>(It.IsAny<Guid>(), It.IsAny<CancellationToken>())).ReturnsAsync((ZoneDto?)null);
        var handler = new CreateZoneHandler(repository.Object, unitOfWork.Object);

        var result = await handler.Handle(
            new CreateZoneCommand(Guid.NewGuid(), new CreateZoneDto { Name = "Downtown", Code = "DT", CityId = 1 }),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Zone created but could not be retrieved.");
    }

    [Fact]
    public async Task UpdateZoneHandler_Should_ThrowNotFound_When_ZoneDoesNotExist()
    {
        var repository = new Mock<IZoneRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var id = Guid.NewGuid();
        repository.Setup(x => x.GetByIdAsync<Zone>(id, It.IsAny<CancellationToken>())).ReturnsAsync((Zone?)null);
        var handler = new UpdateZoneHandler(repository.Object, Mock.Of<ILogger<UpdateZoneHandler>>(), unitOfWork.Object);

        var act = () => handler.Handle(
            new UpdateZoneCommand(Guid.NewGuid(), new UpdateZoneDto { ZoneId = id, Name = "New", Code = "N", CityId = 2 }),
            CancellationToken.None);

        await act.Should().ThrowAsync<EntityNotFoundException>().Where(x => x.ErrorCode == "Zone_NOT_FOUNDED");
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task UpdateZoneHandler_Should_UpdateAndCommit_When_ZoneExists()
    {
        var zone = Zone.Create("Old", "O", 1);
        var repository = new Mock<IZoneRepository>();
        var unitOfWork = SuccessfulUnitOfWork();
        repository.Setup(x => x.GetByIdAsync<Zone>(zone.Id, It.IsAny<CancellationToken>())).ReturnsAsync(zone);
        var handler = new UpdateZoneHandler(repository.Object, Mock.Of<ILogger<UpdateZoneHandler>>(), unitOfWork.Object);

        var result = await handler.Handle(
            new UpdateZoneCommand(Guid.NewGuid(), new UpdateZoneDto { ZoneId = zone.Id, Name = " New ", Code = " N ", CityId = 2 }),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        zone.Name.Should().Be("New");
        zone.Code.Should().Be("N");
        repository.Verify(x => x.Update(zone), Times.Once);
    }

    [Fact]
    public async Task GetZoneByIdHandler_Should_ReturnNotFound_When_ZoneDoesNotExist()
    {
        var repository = new Mock<IZoneRepository>();
        var id = Guid.NewGuid();
        repository.Setup(x => x.GetByIdAsync<ZoneDto>(id, It.IsAny<CancellationToken>())).ReturnsAsync((ZoneDto?)null);
        var handler = new GetZoneByIdHandler(repository.Object);

        var result = await handler.Handle(new GetZoneByIdQuery(id), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        result.Error!.Message.Should().Be("Zone not found.");
    }

    [Fact]
    public async Task GetZonesByCityHandler_Should_PreservePagination_When_RepositoryReturnsPage()
    {
        var repository = new Mock<IZoneRepository>();
        var filter = new ZoneFilterDto { PageNumber = 2, PageSize = 3, SearchTerm = "north" };
        var items = new List<ZoneDto> { new() { Id = Guid.NewGuid(), Name = "North" } };
        repository.Setup(x => x.GetByCityIdAsync(7, filter, It.IsAny<CancellationToken>())).ReturnsAsync((items, 10));
        var handler = new GetZonesByCityHandler(repository.Object);

        var result = await handler.Handle(new GetZonesByCityQuery(7, filter), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Data.Should().BeEquivalentTo(items);
        result.Data.TotalCount.Should().Be(10);
        result.Data.PageNumber.Should().Be(2);
        result.Data.PageSize.Should().Be(3);
    }

    private static Mock<IUnitOfWork> SuccessfulUnitOfWork()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        return unitOfWork;
    }
}
