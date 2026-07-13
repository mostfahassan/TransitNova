using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using TransitNova.Api.Controllers;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.Api.IntegrationTests;

public sealed class ResultExtensionsTests
{
    [Fact]
    public void ToActionResult_BaseNoContent_Should_ReturnEmptyNoContentResult()
    {
        var result = BaseResult.NoContent();

        var actionResult = result.ToActionResult();

        actionResult.Should().BeOfType<NoContentResult>();
        ((NoContentResult)actionResult).StatusCode.Should().Be(204);
    }

    [Fact]
    public void ToActionResult_GenericNoContent_Should_ReturnEmptyNoContentResult()
    {
        var result = new Result<object?>(null, true, ResultStatus.NoContent);

        var actionResult = result.ToActionResult();

        actionResult.Should().BeOfType<NoContentResult>();
        ((NoContentResult)actionResult).StatusCode.Should().Be(204);
    }

    [Fact]
    public void ToActionResult_UnexpectedFailure_Should_ReturnInternalServerErrorObjectResult()
    {
        var result = BaseResult.UnExpected(Errors.FailedOperation("database unavailable"));

        var actionResult = result.ToActionResult();

        var objectResult = actionResult.Should().BeOfType<ObjectResult>().Subject;
        objectResult.StatusCode.Should().Be(500);
        objectResult.Value.Should().NotBeNull();
    }
}