using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Carrier;
using TransitNova.BusinessLayer.Features.Carriers.Handlers.ApplyQueries;
using TransitNova.BusinessLayer.Features.Carriers.Queries.Carrier;
using TransitNova.BusinessLayer.Interfaces.Services.CarrierDashboard;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.ApplicationLayer.Tests.Queries.Dashboards;

public sealed class CarrierDashboardQueryHandlerTests
{
    [Fact]
    public async Task Handle_Should_ReturnDashboardResult_When_ServiceSucceedsAsync()
    {
        var carrierId = Guid.NewGuid();
        var dashboard = new CarrierDashboardDto
        {
           
        };
        var dashboardService = new Mock<ICarrierDashboard>();
        dashboardService.Setup(x => x.BuildAsync(carrierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CarrierDashboardDto>.Success(dashboard));
        var handler = new GetCarrierDashboardHandler(dashboardService.Object, Mock.Of<ILogger<GetCarrierDashboardHandler>>());

        var result = await handler.Handle(new GetCarrierDashboardQuery(carrierId), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeSameAs(dashboard);
      
    }

    [Fact]
    public async Task Handle_Should_ReturnNotFoundResult_When_ServiceReturnsNotFoundAsync()
    {
        var carrierId = Guid.NewGuid();
        var dashboardService = new Mock<ICarrierDashboard>();
        dashboardService.Setup(x => x.BuildAsync(carrierId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Result<CarrierDashboardDto>.NotFound(Errors.CarrierNotFound("Carrier profile not found.")));
        var handler = new GetCarrierDashboardHandler(dashboardService.Object, Mock.Of<ILogger<GetCarrierDashboardHandler>>());

        var result = await handler.Handle(new GetCarrierDashboardQuery(carrierId), CancellationToken.None);

        result.Status.Should().Be(ResultStatus.NotFound);
        result.Error!.Message.Should().Be("Carrier profile not found.");
    }

    [Fact]
    public async Task Handle_Should_ForwardCarrierIdAndCancellationToken_ToDashboardServiceAsync()
    {
        var carrierId = Guid.NewGuid();
        using var cancellation = new CancellationTokenSource();
        var dashboardService = new Mock<ICarrierDashboard>();
        dashboardService.Setup(x => x.BuildAsync(carrierId, cancellation.Token))
            .ReturnsAsync(Result<CarrierDashboardDto>.Success(new CarrierDashboardDto()));
        var handler = new GetCarrierDashboardHandler(dashboardService.Object, Mock.Of<ILogger<GetCarrierDashboardHandler>>());

        await handler.Handle(new GetCarrierDashboardQuery(carrierId), cancellation.Token);

        dashboardService.Verify(x => x.BuildAsync(carrierId, cancellation.Token), Times.Once);
        dashboardService.VerifyNoOtherCalls();
    }
}
