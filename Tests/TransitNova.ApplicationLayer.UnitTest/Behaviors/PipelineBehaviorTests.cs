using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using System.Text.Json;
using TransitNova.BusinessLayer.Common.Behaviors;
using TransitNova.BusinessLayer.Common.Caching;
using TransitNova.BusinessLayer.Common.CQRS;
using TransitNova.BusinessLayer.Common.Interfaces;
using TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces;
using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.Interfaces.Repositories.IdempotentRepository;
using TransitNova.BusinessLayer.Interfaces.Services.CacheService;
using TransitNova.BusinessLayer.Interfaces.Services.UnitOfWork;

namespace TransitNova.ApplicationLayer.Tests.Behaviors;

public sealed class PipelineBehaviorTests
{
    [Fact]
    public async Task ValidationBehavior_Should_CallNext_When_CommandIsValidAsync()
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
    public async Task ValidationBehavior_Should_ReturnValidationFailure_When_CommandIsInvalidAsync()
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
    public async Task ValidationBehavior_Should_DeduplicateErrors_When_ValidatorsReturnSameErrorAsync()
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
    public async Task ValidationBehavior_Should_BypassValidators_When_RequestIsQueryAsync()
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
    public async Task CachingBehavior_Should_ReturnCachedResponse_When_CacheHitOccursAsync()
    {
        var cached = BaseResult.Success();
        var cancellationToken = CancellationToken.None;
        var cache = new Mock<ICacheService>();
        cache.Setup(x => x.GetAsync<BaseResult>("test:key", cancellationToken)).ReturnsAsync(cached);
        var behavior = new CachingBehavior<CacheableRequest, BaseResult>(cache.Object);
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
    public async Task CachingBehavior_Should_ExecuteAndCacheResponse_When_CacheMissOccursAsync()
    {
        var response = BaseResult.Success();

        var cancellationToken = CancellationToken.None;
        var cache = new Mock<ICacheService>();
        cache.Setup(x => x.GetAsync<BaseResult>("test:key", cancellationToken)).ReturnsAsync((BaseResult?)null);
        var behavior = new CachingBehavior<CacheableRequest, BaseResult>(cache.Object);

        var result = await behavior.Handle(
            new CacheableRequest("test:key"),
            _ => Task.FromResult(response),
            CancellationToken.None);

        result.Should().BeSameAs(response);
        cache.Verify(x => x.SetAsync("test:key", response, TimeSpan.FromMinutes(20), cancellationToken), Times.Once);
    }

    [Fact]
    public async Task CachingBehavior_Should_BypassCache_When_RequestIsNotCacheableAsync()
    {
        var cache = new Mock<ICacheService>();
        var behavior = new CachingBehavior<PlainRequest, BaseResult>(cache.Object);

        var result = await behavior.Handle(
            new PlainRequest(),
            _ => Task.FromResult(BaseResult.Success()),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task CachingBehavior_WhenHandlerThrows_ShouldNotCacheResponseAsync()
    {
        var cache = new Mock<ICacheService>();
        var cancellationToken = CancellationToken.None;

        cache.Setup(x => x.GetAsync<BaseResult>("test:key", cancellationToken)).ReturnsAsync((BaseResult?)null);
        var behavior = new CachingBehavior<CacheableRequest, BaseResult>(cache.Object);

        var act = () => behavior.Handle(
            new CacheableRequest("test:key"),
            _ => throw new InvalidOperationException("handler failed"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("handler failed");
        cache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<BaseResult>(), It.IsAny<TimeSpan>(), cancellationToken), Times.Never);
    }

    [Fact]
    public async Task CachingBehavior_Should_ForwardCancellationTokenToCacheAndHandler_When_TokenIsProvidedAsync()
    {
        var cache = new Mock<ICacheService>();
        using var cancellation = new CancellationTokenSource();
        cache.Setup(x => x.GetAsync<BaseResult>("test:key", cancellation.Token)).ReturnsAsync((BaseResult?)null);
        var behavior = new CachingBehavior<CacheableRequest, BaseResult>(cache.Object);
        var received = CancellationToken.None;
        var response = BaseResult.Success();

        await behavior.Handle(
            new CacheableRequest("test:key"),
            token =>
            {
                received = token;
                return Task.FromResult(response);
            },
            cancellation.Token);

        received.Should().Be(cancellation.Token);
        cache.Verify(x => x.GetAsync<BaseResult>("test:key", cancellation.Token), Times.Once);
        cache.Verify(x => x.SetAsync(
            "test:key",
            response,
            TimeSpan.FromMinutes(20),
            cancellation.Token), Times.Once);
    }
    [Fact]
    public async Task CacheInvalidationBehavior_Should_RemoveDeclaredKeys_When_ResponseSucceedsAsync()
    {
        var cache = new Mock<ICacheService>();
        var request = new CacheInvalidatingRequest();
        CacheInvalidationContext.Set(request, "cache:key:1", "cache:key:2", "cache:key:1");
        var behavior = new CacheInvalidationBehavior<CacheInvalidatingRequest, BaseResult>(
            cache.Object,
            NullLogger<CacheInvalidationBehavior<CacheInvalidatingRequest, BaseResult>>.Instance);

        var result = await behavior.Handle(
            request,
            _ => Task.FromResult(BaseResult.Success()),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        cache.Verify(x => x.RemoveAsync("cache:key:1"), Times.Once);
        cache.Verify(x => x.RemoveAsync("cache:key:2"), Times.Once);
    }

    [Fact]
    public async Task CacheInvalidationBehavior_Should_NotRemoveKeys_When_ResponseFailsAsync()
    {
        var cache = new Mock<ICacheService>();
        var request = new CacheInvalidatingRequest();
        CacheInvalidationContext.Set(request, "cache:key:1");
        var behavior = new CacheInvalidationBehavior<CacheInvalidatingRequest, BaseResult>(
            cache.Object,
            NullLogger<CacheInvalidationBehavior<CacheInvalidatingRequest, BaseResult>>.Instance);

        var result = await behavior.Handle(
            request,
            _ => Task.FromResult(BaseResult.Failure(Errors.FailedOperation("failed"))),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        cache.Verify(x => x.RemoveAsync(It.IsAny<string>()), Times.Never);
    }

[Fact]
    public async Task IdempotencyBehavior_Should_ReturnStoredResponseAndSkipNext_When_RequestAlreadyExistsAsync()
    {
        var requestId = Guid.NewGuid();
        var repository = new Mock<IIdempotentRepository>();
        repository.Setup(x => x.ReturnRequestIfExistsAsync(requestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(Serialize(BaseResult.Success()));
        var behavior = CreateIdempotentBehavior(repository);
        var nextCalls = 0;

        var result = await behavior.Handle(
            new TestCommand(requestId, "test"),
            _ =>
            {
                nextCalls++;
                return Task.FromResult(BaseResult.Failure(Errors.FailedOperation("should not run")));
            },
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        nextCalls.Should().Be(0);
        repository.Verify(x => x.CreateRequestAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task IdempotencyBehavior_Should_RecordAndExecute_When_RequestIsNewAsync()
    {
        var requestId = Guid.NewGuid();
        var repository = new Mock<IIdempotentRepository>();
        var unitOfWork = new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);
        var behavior = CreateIdempotentBehavior(repository, unitOfWork);
        var expected = BaseResult.Success();

        var result = await behavior.Handle(
            new TestCommand(requestId, "test"),
            _ => Task.FromResult(expected),
            CancellationToken.None);

        result.Should().BeSameAs(expected);
        repository.Verify(x => x.CreateRequestAsync(
            requestId,
            nameof(TestCommand),
            It.Is<string>(json => JsonSerializer.Deserialize<BaseResult>(json, JsonOptions)!.IsSuccess),
            It.IsAny<CancellationToken>()), Times.Once);
        unitOfWork.Verify(x => x.SaveChangesAsync(CancellationToken.None), Times.Once);
        unitOfWork.Verify(x => x.BeginTransactionAsync(It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
        unitOfWork.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task IdempotentCommandPipelineBehavior_WhenHandlerThrows_ShouldPropagateWithoutRecordingRequestAsync()
    {
        var requestId = Guid.NewGuid();
        var repository = new Mock<IIdempotentRepository>();
        var behavior = CreateIdempotentBehavior(repository);

        var act = () => behavior.Handle(
            new TestCommand(requestId, "test"),
            _ => throw new InvalidOperationException("handler failed"),
            CancellationToken.None);

        await act.Should().ThrowAsync<InvalidOperationException>().WithMessage("handler failed");
        repository.Verify(x => x.CreateRequestAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task IdempotentCommandPipelineBehavior_WhenCancellationTokenIsPassed_ShouldForwardItToRepositoryAndHandlerAsync()
    {
        var requestId = Guid.NewGuid();
        var repository = new Mock<IIdempotentRepository>();
        using var cancellation = new CancellationTokenSource();
        var behavior = CreateIdempotentBehavior(repository);
        var handlerToken = CancellationToken.None;

        await behavior.Handle(
            new TestCommand(requestId, "test"),
            token =>
            {
                handlerToken = token;
                return Task.FromResult(BaseResult.Success());
            },
            cancellation.Token);

        repository.Verify(x => x.ReturnRequestIfExistsAsync(requestId, cancellation.Token), Times.Once);
        repository.Verify(x => x.CreateRequestAsync(
            requestId,
            nameof(TestCommand),
            It.Is<string>(json => JsonSerializer.Deserialize<BaseResult>(json, JsonOptions)!.IsSuccess),
            cancellation.Token), Times.Once);
        handlerToken.Should().Be(cancellation.Token);
    }

    [Fact]
    public async Task IdempotencyBehavior_Should_ThrowJsonException_When_StoredResponseIsCorruptedAsync()
    {
        var requestId = Guid.NewGuid();
        var repository = new Mock<IIdempotentRepository>();
        repository.Setup(x => x.ReturnRequestIfExistsAsync(requestId, It.IsAny<CancellationToken>()))
            .ReturnsAsync("not-json");
        var behavior = CreateIdempotentBehavior(repository);

        var act = () => behavior.Handle(
            new TestCommand(requestId, "test"),
            _ => Task.FromResult(BaseResult.Success()),
            CancellationToken.None);

        await act.Should().ThrowAsync<JsonException>();
        repository.Verify(x => x.CreateRequestAsync(
            It.IsAny<Guid>(),
            It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public void IdempotentCommand_Should_ImplementTransactionalMarker()
    {
        var request = new TestCommand(Guid.NewGuid(), "test");

        request.Should().BeAssignableTo<ITransactional>();
    }

    [Fact]
    public async Task TransactionBehavior_Should_WrapIdempotentCommands_When_RequestSucceedsAsync()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var behavior = new TransactionPipelineBehavior<TestCommand, BaseResult>(unitOfWork.Object);

        var result = await behavior.Handle(
            new TestCommand(Guid.NewGuid(), "test"),
            _ => Task.FromResult(BaseResult.Success()),
            CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        unitOfWork.Verify(x => x.BeginTransactionAsync(CancellationToken.None), Times.Once);
        unitOfWork.Verify(x => x.CommitAsync(CancellationToken.None), Times.Once);
        unitOfWork.Verify(x => x.RollbackAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task TransactionBehavior_Should_BypassTransaction_When_RequestIsNotTransactionalAsync()
    {
        var unitOfWork = new Mock<IUnitOfWork>();
        var behavior = new TransactionPipelineBehavior<PlainRequest, BaseResult>(unitOfWork.Object);

        var result = await behavior.Handle(new PlainRequest(), _ => Task.FromResult(BaseResult.Success()), CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        unitOfWork.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task TransactionBehavior_Should_Commit_When_TransactionalRequestSucceedsAsync()
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
    public async Task TransactionBehavior_Should_Rollback_When_TransactionalRequestReturnsFailureAsync()
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
    public async Task TransactionBehavior_Should_RollbackAndRethrow_When_HandlerThrowsAsync()
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

    private sealed record TestCommand(Guid RequestId, string Name) : IdempotentCommand<BaseResult>(RequestId);
    private sealed record TestQuery(string Name) : IQuery<BaseResult>;
    private sealed record CacheableRequest(string CacheKey) : IRequest<BaseResult>, ICachable;
    private sealed record PlainRequest : IRequest<BaseResult>;
    private sealed record CacheInvalidatingRequest : IRequest<BaseResult>, ICacheInvalidator;
    private sealed record TransactionalRequest : IRequest<BaseResult>, ITransactional;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private static string Serialize<T>(T value) => JsonSerializer.Serialize(value, JsonOptions);
    private static IdempotentCommandPipelineBehavior<TestCommand, BaseResult> CreateIdempotentBehavior(
      Mock<IIdempotentRepository> repository,
      Mock<IUnitOfWork>? unitOfWork = null)
    {
        unitOfWork ??= new Mock<IUnitOfWork>();
        unitOfWork.Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        return new IdempotentCommandPipelineBehavior<TestCommand, BaseResult>(
            unitOfWork.Object,
            repository.Object,
            NullLogger<IdempotentCommandPipelineBehavior<TestCommand, BaseResult>>.Instance);
    }
}





