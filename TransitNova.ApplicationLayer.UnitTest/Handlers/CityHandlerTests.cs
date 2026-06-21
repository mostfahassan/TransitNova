using FluentAssertions;
using Moq;
using TransitNova.BusinessLayer.DTOs.City;
using TransitNova.BusinessLayer.Features.Location.Cities.Commands;
using TransitNova.BusinessLayer.Features.Location.Cities.Handlers.ApplyCommands;
using TransitNova.BusinessLayer.Features.Location.Cities.Handlers.ApplyingQueries;
using TransitNova.BusinessLayer.Features.Location.Cities.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.LocationRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.ApplicationLayer.Tests.Handlers;

public sealed class CityHandlerTests
{
    [Fact]
    public async Task Handle_Should_ReturnCity_When_CityExists()
    {
        var repository = new Mock<ICityRepository>();
        var dto = new CityDto { Id = 5, Name = "Cairo", GovernmentName = "Cairo" };
        repository.Setup(x => x.GetByIdAsync<CityDto>(5, It.IsAny<CancellationToken>())).ReturnsAsync(dto);
        var handler = new GetCityByIdHandler(repository.Object);

        var result = await handler.Handle(new GetCityByIdQuery(5), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeSameAs(dto);
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFound_When_CityDoesNotExist()
    {
        var repository = new Mock<ICityRepository>();
        repository.Setup(x => x.GetByIdAsync<CityDto>(9, It.IsAny<CancellationToken>())).ReturnsAsync((CityDto?)null);
        var handler = new GetCityByIdHandler(repository.Object);

        var result = await handler.Handle(new GetCityByIdQuery(9), CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Data.Should().BeNull();
        result.Error!.Message.Should().Be("City not found.");
    }

    [Fact]
    public async Task Handle_Should_CreateCityAndInvalidateGovernmentCache_When_CommandIsValid()
    {
        var repository = new Mock<ICityRepository>();
        var cache = new Mock<ICacheService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        repository.Setup(x => x.AddAsync(It.IsAny<City>(), It.IsAny<CancellationToken>()))
            .Callback<City, CancellationToken>((city, _) => city.Government = Government.Create("Cairo", 1))
            .Returns(Task.CompletedTask);
        var handler = new CreateCityHandler(repository.Object, cache.Object, unitOfWork.Object);
        var command = new CreateCityCommand(Guid.NewGuid(), new CreateCityDto { Name = " Nasr City ", GovernmentId = 1 });

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Nasr City");
        result.Data.GovernmentName.Should().Be("Cairo");
        repository.Verify(x => x.AddAsync(It.Is<City>(c => c.Name == "Nasr City"), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        cache.Verify(x => x.RemoveAsync(CacheKeys.CitiesByGovernment(1)), Times.Once);
    }
}
