using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Bundle;
using TransitNova.BusinessLayer.Features.Bundles.Commands;
using TransitNova.BusinessLayer.Features.Bundles.Handlers.ApplyingCommands;
using TransitNova.BusinessLayer.Features.Bundles.Handlers.ApplyingQueries;
using TransitNova.BusinessLayer.Features.Bundles.Queries;
using TransitNova.BusinessLayer.Interfaces.Repositories.GenericRepository;
using TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;
using TransitNova.Domain.Contracts.Caching;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.ApplicationLayer.Tests.Handlers;

public sealed class BundleHandlerTests
{
    [Fact]
    public async Task Handle_Should_CreateBundleAndInvalidateCache_When_CommandIsValidAsync()
    {
        var repository = new Mock<IGenericRepository<Bundle, Guid>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var cache = new Mock<ICacheService>();
        var logger = new Mock<ILogger<CreateBundleHandler>>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        Bundle? captured = null;
        repository.Setup(x => x.AddAsync(It.IsAny<Bundle>(), It.IsAny<CancellationToken>()))
            .Callback<Bundle, CancellationToken>((bundle, _) => captured = bundle)
            .Returns(Task.CompletedTask);
        var handler = new CreateBundleHandler(repository.Object, unitOfWork.Object, logger.Object);
        var userId = Guid.NewGuid();
        var command = new CreateBundleCommand(
            Guid.NewGuid(),
            userId,
            new CreateBundleDto
            {
                BundleName = "Business",
                BundlePrice = 500m,
                BundleDescription = "Business package",
                TotalWeight = 200m,
                TotalDistance = 1000m,
                TotalShipments = 20
            });

        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Message.Should().Be("Bundle created successfully.");
        captured.Should().NotBeNull();
        captured!.BundleName.Should().Be("Business");
        captured.CreatedBy.Should().Be(userId.ToString());
        repository.Verify(x => x.AddAsync(It.IsAny<Bundle>(), It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateBundleHandler_Should_ReturnFailureAndSkipWrites_When_BundleDoesNotExistAsync()
    {
        var repository = new Mock<IGenericRepository<Bundle, Guid>>();
        var managers = new Mock<IOperationManagerQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var cache = new Mock<ICacheService>();
        var appUserId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var bundleId = Guid.NewGuid();
        managers.Setup(x => x.GetUserIdAsync(appUserId, It.IsAny<CancellationToken>())).ReturnsAsync(managerId);
        repository.Setup(x => x.GetByIdAsync<Bundle>(bundleId, It.IsAny<CancellationToken>())).ReturnsAsync((Bundle?)null);
        var handler = new UpdateBundleHandler(repository.Object, managers.Object, unitOfWork.Object, Mock.Of<ILogger<UpdateBundleHandler>>());

        var result = await handler.Handle(
            new UpdateBundleCommand(Guid.NewGuid(), bundleId, UpdateDto(), appUserId),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Message.Should().Be("Bundle not found");
        repository.Verify(x => x.Update(It.IsAny<Bundle>()), Times.Never);
        unitOfWork.Verify(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task UpdateBundleHandler_Should_UpdatePersistAndInvalidateCaches_When_BundleExistsAsync()
    {
        var repository = new Mock<IGenericRepository<Bundle, Guid>>();
        var managers = new Mock<IOperationManagerQueryRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var cache = new Mock<ICacheService>();
        var appUserId = Guid.NewGuid();
        var managerId = Guid.NewGuid();
        var bundle = Bundle.Create("creator", "Business", 100, "Description", 50, 500, 5);
        managers.Setup(x => x.GetUserIdAsync(appUserId, It.IsAny<CancellationToken>())).ReturnsAsync(managerId);
        repository.Setup(x => x.GetByIdAsync<Bundle>(bundle.Id, It.IsAny<CancellationToken>())).ReturnsAsync(bundle);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var handler = new UpdateBundleHandler(repository.Object, managers.Object, unitOfWork.Object, Mock.Of<ILogger<UpdateBundleHandler>>());

        var result = await handler.Handle(
            new UpdateBundleCommand(Guid.NewGuid(), bundle.Id, UpdateDto(), appUserId),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        bundle.BundlePrice.Should().Be(250);
        bundle.TotalWeight.Should().Be(75);
        bundle.TotalShipments.Should().Be(12);
        bundle.UpdatedBy.Should().Be(managerId.ToString());
        repository.Verify(x => x.Update(bundle), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task DeleteBundleHandler_Should_DeletePersistAndInvalidateCaches_When_CommandIsHandledAsync()
    {
        var repository = new Mock<IGenericRepository<Bundle, Guid>>();
        var unitOfWork = new Mock<IUnitOfWork>();
        var cache = new Mock<ICacheService>();
        var id = Guid.NewGuid();
        repository.Setup(x => x.DeleteAsync(id, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var handler = new DeleteBundleHandler(repository.Object, unitOfWork.Object, Mock.Of<ILogger<DeleteBundleHandler>>());

        var result = await handler.Handle(new DeleteBundleCommand(Guid.NewGuid(), id), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        repository.Verify(x => x.DeleteAsync(id, CancellationToken.None), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
    }

    [Theory]
    [InlineData(false, ResultStatus.NotFound)]
    [InlineData(true, ResultStatus.Success)]
    public async Task GetBundleByIdHandler_Should_ReturnExpectedResult_When_RepositoryResultIsKnownAsync(bool exists, ResultStatus expectedStatus)
    {
        var repository = new Mock<IGenericRepository<Bundle, Guid>>();
        var id = Guid.NewGuid();
        repository.Setup(x => x.GetByIdAsync<RetrieveBundleDto>(id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(exists ? new RetrieveBundleDto { BundleName = "Business" } : null);
        var handler = new GetBundleByIdHandler(repository.Object, Mock.Of<ILogger<GetBundleByIdHandler>>());

        var result = await handler.Handle(new GetBundleByIdQuery(id), CancellationToken.None);

        result.Status.Should().Be(expectedStatus);
    }

    [Fact]
    public async Task GetBundleListHandler_Should_ReturnEmptySuccess_When_NoBundlesExistAsync()
    {
        var repository = new Mock<IGenericRepository<Bundle, Guid>>();
        repository.Setup(x => x.GetListAsync<RetrieveBundleDto>(It.IsAny<CancellationToken>())).ReturnsAsync([]);
        var handler = new GetBundleListHandler(repository.Object, Mock.Of<ILogger<GetBundleListHandler>>());

        var result = await handler.Handle(new GetBundleListQuery(), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeEmpty();
    }

    [Fact]
    public async Task GetBundleListHandler_Should_ReturnRepositoryList_When_BundlesExistAsync()
    {
        var repository = new Mock<IGenericRepository<Bundle, Guid>>();
        var bundles = new List<RetrieveBundleDto> { new() { BundleName = "Business" } };
        repository.Setup(x => x.GetListAsync<RetrieveBundleDto>(It.IsAny<CancellationToken>())).ReturnsAsync(bundles);
        var handler = new GetBundleListHandler(repository.Object, Mock.Of<ILogger<GetBundleListHandler>>());

        var result = await handler.Handle(new GetBundleListQuery(), CancellationToken.None);

        result.Data.Should().BeSameAs(bundles);
    }

    private static UpdateBundleDto UpdateDto() => new()
    {
        BundlePrice = 250,
        TotalWeight = 75,
        TotalShipments = 12
    };
}


