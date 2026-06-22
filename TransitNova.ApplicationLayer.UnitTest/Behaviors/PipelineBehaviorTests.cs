using FluentAssertions;
using FluentValidation;
using MediatR;
using Moq;
using TransitNova.BusinessLayer.Common.Behaviors;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Interfaces.Repositories.IdempotentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;

namespace TransitNova.ApplicationLayer.Tests.Behaviors;

public sealed class PipelineBehaviorTests
{
    [Fact]
    public async Task ValidationBehavior_Should_CallNext_When_CommandIsValid()
    {
        var validator = new InlineValidator<TestCommand>();
        validator.RuleFor(x => x.Name).NotEmpty();
        var behavior = new ValidationBehavior<TestCommand, BaseResult>([validator]);
        var nextCalls = 0;

        var result = await behavior.Handle(
            new TestCommand(Guid.NewGuid(), "valid"),
            _ =>
            {
                nextCalls++;
                return Task.FromResult(BaseResult.Success());
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        nextCalls.Should().Be(1);
    }

    [Fact]
    public async Task ValidationBehavior_Should_ReturnValidationFailure_When_CommandIsInvalid()
    {
        var validator = new InlineValidator<TestCommand>();
        validator.RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        var behavior = new ValidationBehavior<TestCommand, BaseResult>([validator]);
        var nextCalls = 0;

        var result = await behavior.Handle(
            new TestCommand(Guid.NewGuid(), string.Empty),
            _ =>
            {
                nextCalls++;
                return Task.FromResult(BaseResult.Success());
            },
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Errors.Should().ContainSingle(e => e.Message == "Name is required.");
        nextCalls.Should().Be(0);
    }

    [Fact]
    public async Task ValidationBehavior_Should_DeduplicateErrors_When_ValidatorsReturnSameError()
    {
        var first = new InlineValidator<TestCommand>();
        first.RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        var second = new InlineValidator<TestCommand>();
        second.RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        var behavior = new ValidationBehavior<TestCommand, BaseResult>([first, second]);

        var result = await behavior.Handle(
            new TestCommand(Guid.NewGuid(), string.Empty),
            _ => Task.FromResult(BaseResult.Success()),
            CancellationToken.None);

        result.Errors.Should().ContainSingle();
    }

    [Fact]
    public async Task ValidationBehavior_Should_BypassValidators_When_RequestIsQuery()
    {
        var validator = new InlineValidator<TestQuery>();
        validator.RuleFor(x => x.Name).NotEmpty();
        var behavior = new ValidationBehavior<TestQuery, BaseResult>([validator]);

        var result = await behavior.Handle(
            new TestQuery(string.Empty),
            _ => Task.FromResult(BaseResult.Success()),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public async Task CachingBehavior_Should_ReturnCachedResponse_When_CacheHitOccurs()
    {
        var cached = BaseResult.Success();
        var cancellationToken = CancellationToken.None;
        var cache = new Mock<ICacheService>();
        cache.Setup(x => x.GetAsync<BaseResult>("test:key", cancellationToken)).ReturnsAsync(cached);
        var behavior = new CachingBehaviour<CacheableRequest, BaseResult>(cache.Object);
        var nextCalls = 0;

        var result = await behavior.Handle(
            new CacheableRequest("test:key"),
            _ =>
            {
                nextCalls++;
                return Task.FromResult(BaseResult.Failure(Errors.FailedOperation("should not execute")));
            },
            CancellationToken.None);

        result.Should().BeSameAs(cached);
        nextCalls.Should().Be(0);
        cache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<BaseResult>(), It.IsAny<TimeSpan>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task CachingBehavior_Should_ExecuteAndCacheResponse_When_CacheMissOccurs()
    {
        var response = BaseResult.Success();

        var cancellationToken = CancellationToken.None;
        var cache = new Mock<ICacheService>();
        cache.Setup(x => x.GetAsync<BaseResult>("test:key", cancellationToken)).ReturnsAsync((BaseResult?)null);
        var behavior = new CachingBehaviour<CacheableRequest, BaseResult>(cache.Object);

        var result = await behavior.Handle(
            new CacheableRequest("test:key"),
            _ => Task.FromResult(response),
            CancellationToken.None);

        result.Should().BeSameAs(response);
        cache.Verify(x => x.SetAsync("test:key", response, TimeSpan.FromMinutes(20), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CachingBehavior_Should_BypassCache_When_RequestIsNotCacheable()
    {
        var cache = new Mock<ICacheService>();
        var behavior = new CachingBehaviour<PlainRequest, BaseResult>(cache.Object);

        var result = await behavior.Handle(
            new PlainRequest(),
            _ => Task.FromResult(BaseResult.Success()),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CachingBehaviour_WhenHandlerThrows_ShouldNotCacheResponse()
    {
        var cache = new Mock<ICacheService>(); 
        var cancellationToken = CancellationToken.None;

        cache.Setup(x => x.GetAsync<BaseResult>("test:key", cancellationToken)).ReturnsAsync((BaseResult?)null);
        var behavior = new CachingBehaviour<CacheableRequest, BaseResult>(cache.Object);

        var act = () => behavior.Handle(
            new CacheableRequest("test:key"),
            _ => throw new InvalidOperationException("handler failed"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("handler failed");
        cache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<BaseResult>(), It.IsAny<TimeSpan>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task CachingBehaviour_WhenCancellationTokenIsPassed_ShouldInvokeHandlerWithDelegateDefaultToken()
    {
        var cache = new Mock<ICacheService>();
        var cancellationToken = CancellationToken.None;
        cache.Setup(x => x.GetAsync<BaseResult>("test:key", cancellationToken)).ReturnsAsync((BaseResult?)null);
        var behavior = new CachingBehaviour<CacheableRequest, BaseResult>(cache.Object);
        using var cancellation = new CancellationTokenSource();
        var received = CancellationToken.None;

        await behavior.Handle(
            new CacheableRequest("test:key"),
            token =>
            {
                received = token;
                return Task.FromResult(BaseResult.Success());
            },
            cancellation.Token);

        received.Should().Be(CancellationToken.None,
            "the current behavior invokes next() without passing its pipeline token");
    }

    [Fact]
    public async Task IdempotencyBehavior_Should_ThrowConflictAndSkipNext_When_RequestAlreadyExists()
    {
        var requestId = Guid.NewGuid();
        var repository = new Mock<IIdempotentRepository>();
        repository.Setup(x => x.RequestExistsAsync(requestId, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        var behavior = new IdempotentCommandPipelineBehaviour<TestCommand, BaseResult>(repository.Object);
        var nextCalls = 0;

        var act = () => behavior.Handle(
            new TestCommand(requestId, "test"),
            _ =>
            {
                nextCalls++;
                return Task.FromResult(BaseResult.Success());
            },
            CancellationToken.None);

        await act.Should().ThrowAsync<BusinessLayer.Common.Exceptions.ReusedRefreshTokenException.IdempotentConflicExceptionException>();
        nextCalls.Should().Be(0);
        repository.Verify(x => x.CreateRequestAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task IdempotencyBehavior_Should_RecordAndExecute_When_RequestIsNew()
    {
        var requestId = Guid.NewGuid();
        var repository = new Mock<IIdempotentRepository>();
        repository.Setup(x => x.RequestExistsAsync(requestId, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var behavior = new IdempotentCommandPipelineBehaviour<TestCommand, BaseResult>(repository.Object);
        var expected = BaseResult.Success();

        var result = await behavior.Handle(
            new TestCommand(requestId, "test"),
            _ => Task.FromResult(expected),
            CancellationToken.None);

        result.Should().BeSameAs(expected);
        repository.Verify(x => x.CreateRequestAsync(requestId, nameof(TestCommand), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task IdempotentCommandPipelineBehaviour_WhenHandlerThrows_ShouldPropagateAfterRecordingRequest()
    {
        var requestId = Guid.NewGuid();
        var repository = new Mock<IIdempotentRepository>();
        repository.Setup(x => x.RequestExistsAsync(requestId, It.IsAny<CancellationToken>())).ReturnsAsync(false);
        var behavior = new IdempotentCommandPipelineBehaviour<TestCommand, BaseResult>(repository.Object);

        var act = () => behavior.Handle(
            new TestCommand(requestId, "test"),
            _ => throw new InvalidOperationException("handler failed"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("handler failed");
        repository.Verify(x => x.CreateRequestAsync(requestId, nameof(TestCommand), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task IdempotentCommandPipelineBehaviour_WhenCancellationTokenIsPassed_ShouldForwardItToRepositoryAndHandler()
    {
        var requestId = Guid.NewGuid();
        var repository = new Mock<IIdempotentRepository>();
        using var cancellation = new CancellationTokenSource();
        repository.Setup(x => x.RequestExistsAsync(requestId, cancellation.Token)).ReturnsAsync(false);
        var behavior = new IdempotentCommandPipelineBehaviour<TestCommand, BaseResult>(repository.Object);
        var handlerToken = CancellationToken.None;

        await behavior.Handle(
            new TestCommand(requestId, "test"),
            token =>
            {
                handlerToken = token;
                return Task.FromResult(BaseResult.Success());
            },
            cancellation.Token);

        repository.Verify(x => x.RequestExistsAsync(requestId, cancellation.Token), Times.Once);
        repository.Verify(x => x.CreateRequestAsync(requestId, nameof(TestCommand), cancellation.Token), Times.Once);
        handlerToken.Should().Be(cancellation.Token);
    }

    [Fact]
    public async Task TransactionBehavior_Should_BypassTransaction_When_RequestIsNotTransactional()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var behavior = new TransactionPipelineBehavior<PlainRequest, BaseResult>(unitOfWork.Object);

        var result = await behavior.Handle(new PlainRequest(), _ => Task.FromResult(BaseResult.Success()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TransactionBehavior_Should_Commit_When_TransactionalRequestSucceeds()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var behavior = new TransactionPipelineBehavior<TransactionalRequest, BaseResult>(unitOfWork.Object);

        var result = await behavior.Handle(new TransactionalRequest(), _ => Task.FromResult(BaseResult.Success()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        unitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task TransactionBehavior_Should_Rollback_When_TransactionalRequestReturnsFailure()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var behavior = new TransactionPipelineBehavior<TransactionalRequest, BaseResult>(unitOfWork.Object);

        var result = await behavior.Handle(
            new TransactionalRequest(),
            _ => Task.FromResult(BaseResult.Failure(Errors.FailedOperation("failed"))),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        unitOfWork.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task TransactionBehavior_Should_RollbackAndRethrow_When_HandlerThrows()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var behavior = new TransactionPipelineBehavior<TransactionalRequest, BaseResult>(unitOfWork.Object);

        var act = () => behavior.Handle(
            new TransactionalRequest(),
            _ => throw new InvalidOperationException("boom"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("boom");
        unitOfWork.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    private sealed record TestCommand(Guid RequestId, string Name) : IdempotantCommand<BaseResult>(RequestId);
    private sealed record TestQuery(string Name) : IQuery<BaseResult>;
    private sealed record CacheableRequest(string CacheKey) : IRequest<BaseResult>, ICachable;
    private sealed record PlainRequest : IRequest<BaseResult>;
    private sealed record TransactionalRequest : IRequest<BaseResult>, ITransactional;
}
