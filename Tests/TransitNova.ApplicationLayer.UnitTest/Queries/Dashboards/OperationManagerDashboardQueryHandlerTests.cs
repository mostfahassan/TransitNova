using FluentAssertions;
using Moq;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.OperationManager;
using TransitNova.BusinessLayer.Features.OperationManagerService.Handlers.OperationManagerQueryHandler;
using TransitNova.BusinessLayer.Features.OperationManagerService.Queries.OperationManagerQueries;
using TransitNova.BusinessLayer.Interfaces.Services.OperationManagerDashboard;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.ApplicationLayer.Tests.Queries.Dashboards;

public sealed class OperationManagerDashboardQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_ReturnDashboardResult_When_ServiceSucceedsAsync()
    {
        var operationManagerId = Guid.NewGuid();
        var dashboard = new OperationManagerDashboardDto
        {
            TotalShipments = 8,
            PendingShipments = 3,
            DeliveredShipments = 5
        };
        var dashboardService = new Mock<IOperationManagerDashboard>();
        dashboardService.Setup(x => x.BuildAsync(operationManagerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<OperationManagerDashboardDto>.Success(dashboard));
        var handler = new GetOperationManagerDashboardHandler(dashboardService.Object);

        var result = await handler.Handle(new GetOperationManagerDashboardQuery(operationManagerId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeSameAs(dashboard);
        result.Data!.TotalShipments.Should().Be(8);
    }

    [Fact]
    public async Task Handle_Should_ReturnFailureResult_When_ServiceReturnsFailureAsync()
    {
        var operationManagerId = Guid.NewGuid();
        var dashboardService = new Mock<IOperationManagerDashboard>();
        dashboardService.Setup(x => x.BuildAsync(operationManagerId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<OperationManagerDashboardDto>.Failure(Errors.NotFound("Operation manager not found.")));
        var handler = new GetOperationManagerDashboardHandler(dashboardService.Object);

        var result = await handler.Handle(new GetOperationManagerDashboardQuery(operationManagerId), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.Failure);
        result.Error!.Message.Should().Be("Operation manager not found.");
    }

    [Fact]
    public async Task Handle_Should_ForwardOperationManagerIdAndCancellationToken_ToDashboardServiceAsync()
    {
        var operationManagerId = Guid.NewGuid();
        using var cancellation = new CancellationTokenSource();
        var dashboardService = new Mock<IOperationManagerDashboard>();
        dashboardService.Setup(x => x.BuildAsync(operationManagerId, cancellation.Token))
            .ReturnsAsync(Result<OperationManagerDashboardDto>.Success(new OperationManagerDashboardDto()));
        var handler = new GetOperationManagerDashboardHandler(dashboardService.Object);

        await handler.Handle(new GetOperationManagerDashboardQuery(operationManagerId), cancellation.Token);

        dashboardService.Verify(x => x.BuildAsync(operationManagerId, cancellation.Token), Times.Once);
        dashboardService.VerifyNoOtherCalls();
    }
}
