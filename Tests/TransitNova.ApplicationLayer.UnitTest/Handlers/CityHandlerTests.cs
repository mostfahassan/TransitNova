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
    public async Task Handle_Should_ReturnCity_When_CityExistsAsync()
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
    public async Task Handle_Should_ReturnNotFound_When_CityDoesNotExistAsync()
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
    public async Task Handle_Should_CreateCityAndInvalidateGovernmentCache_When_CommandIsValidAsync()
    {
        var repository = new Mock<ICityRepository>();
        var cache = new Mock<ICacheService>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        repository.Setup(x => x.AddAsync(It.IsAny<City>(), It.IsAny<CancellationToken>()));
          
        var handler = new CreateCityHandler(repository.Object, unitOfWork.Object);
        var command = new CreateCityCommand(Guid.NewGuid(), new CreateCityDto { Name = " Nasr City ", GovernmentId = 1 });

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data!.Name.Should().Be("Nasr City");
        result.Data.GovernmentId.Should().Be(1);
        repository.Verify(x => x.AddAsync(It.Is<City>(c => c.Name == "Nasr City"), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}


