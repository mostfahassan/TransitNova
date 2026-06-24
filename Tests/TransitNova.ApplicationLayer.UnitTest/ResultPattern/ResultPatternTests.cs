using FluentAssertions;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.Domain.Enums.Result;

namespace TransitNova.ApplicationLayer.Tests.ResultPattern;

public sealed class ResultPatternTests
{
    [Fact]
    public void Success_Should_CreateSuccessfulResult_When_Called()
    {
        var result = BaseResult.Success();

        result.IsSuccess.Should().BeTrue();
        result.Status.Should().Be(ResultStatus.Success);
        result.Error.Should().BeNull();
    }

    [Fact]
    public void Validation_Should_PreserveAllErrors_When_MultipleErrorsProvided()
    {
        var errors = new[] { Errors.Validation("one"), Errors.Validation("two") };

        var result = BaseResult.Validation(errors);

        result.IsFailure.Should().BeTrue();
        result.Status.Should().Be(ResultStatus.ValidationError);
        result.Errors.Should().Equal(errors);
        result.Error.Should().Be(errors[0]);
    }

    [Fact]
    public void GenericSuccess_Should_PreserveData_When_DataProvided()
    {
        var data = new object();

        var result = Result<object>.Success(data);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().BeSameAs(data);
    }

    [Fact]
    public void GenericNotFound_Should_ReturnNoData_When_ErrorProvided()
    {
        var error = Errors.NotFound("missing");

        var result = Result<string>.NotFound(error);

        result.IsFailure.Should().BeTrue();
        result.Status.Should().Be(ResultStatus.NotFound);
        result.Data.Should().BeNull();
        result.Error.Should().Be(error);
    }
}
