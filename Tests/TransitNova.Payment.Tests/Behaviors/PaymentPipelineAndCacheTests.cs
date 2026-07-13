using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using TransitNovaPayment.Busieness.Common.Abstract.PaymentMethods;
using TransitNovaPayment.Busieness.Common.Behaviour;
using TransitNovaPayment.Busieness.Common.CQRS;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ResultPattern;
using TransitNovaPayment.Busieness.Common.ResultResponse.Result.ErrorsResult.Enum;
using TransitNovaPayment.Busieness.Implementation.Cache;
using TransitNovaPayment.Busieness.Interfaces.Common;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
using InfrastructureCache = TransitNovaPayment.Infrastructure.Service.Caching.MemoryCacheService;

namespace TransitNova.Payment.Tests.Behaviors;

public sealed class PaymentPipelineAndCacheTests
{
    [Fact]
    public async Task ValidationBehavior_Should_BypassValidation_ForNonCommandRequestAsync()
    {
        var validator = new InlineValidator<TestQuery>();
        validator.RuleFor(x => x.Name).NotEmpty();
        var behavior = new ValidationBehavior<TestQuery, BaseResult>([validator]);
        var expected = BaseResult.Unauthorized(new(ErrorCode.UNAUTHORIZED, "expected"));

        var result = await behavior.Handle(
            new TestQuery(string.Empty),
            _ => Task.FromResult(expected),
            CancellationToken.None);

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task ValidationBehavior_Should_CallHandler_WhenNoValidatorsAreRegisteredAsync()
    {
        var behavior = new ValidationBehavior<TestCommand, BaseResult>([]);
        var calls = 0;

        await behavior.Handle(
            new TestCommand("value"),
            _ =>
            {
                calls++;
                return Task.FromResult(BaseResult.Unauthorized(new(ErrorCode.UNAUTHORIZED, "expected")));
            },
            CancellationToken.None);

        calls.Should().Be(1);
    }

    [Fact]
    public async Task ValidationBehavior_Should_DeduplicateFailures_AndSkipHandlerAsync()
    {
        var first = new InlineValidator<TestCommand>();
        first.RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        var second = new InlineValidator<TestCommand>();
        second.RuleFor(x => x.Name).NotEmpty().WithMessage("Name is required.");
        var behavior = new ValidationBehavior<TestCommand, BaseResult>([first, second]);
        var calls = 0;

        var result = await behavior.Handle(
            new TestCommand(string.Empty),
            _ =>
            {
                calls++;
                return Task.FromResult(BaseResult.Unauthorized(new(ErrorCode.UNAUTHORIZED, "unexpected")));
            },
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Message.Should().Contain("Name is required.").And.NotContain("Name is required.,Name is required.");
        calls.Should().Be(0);
    }

    [Fact]
    public async Task ValidationBehavior_Should_CallHandler_WhenCommandIsValidAsync()
    {
        var validator = new InlineValidator<TestCommand>();
        validator.RuleFor(x => x.Name).NotEmpty();
        var behavior = new ValidationBehavior<TestCommand, BaseResult>([validator]);

        var result = await behavior.Handle(
            new TestCommand("valid"),
            _ => Task.FromResult(BaseResult.Unauthorized(new(ErrorCode.UNAUTHORIZED, "expected"))),
            CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        result.Error!.Code.Should().Be(ErrorCode.UNAUTHORIZED);
    }

    [Fact]
    public async Task CachingBehavior_Should_ReturnCachedValue_WithoutExecutingHandlerAsync()
    {
        var cache = new Mock<ICacheService>();
        var expected = new CacheResponse("cached");
        cache.Setup(x => x.GetAsync<CacheResponse>("payment:1", CancellationToken.None)).ReturnsAsync(expected);
        var behavior = new CachingBehavior<CacheableRequest, CacheResponse>(cache.Object);
        var calls = 0;

        var result = await behavior.Handle(
            new CacheableRequest("payment:1"),
            _ =>
            {
                calls++;
                return Task.FromResult(new CacheResponse("fresh"));
            },
            CancellationToken.None);

        result.Should().BeSameAs(expected);
        calls.Should().Be(0);
        cache.Verify(x => x.SetAsync(It.IsAny<string>(), It.IsAny<CacheResponse>(), It.IsAny<TimeSpan>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task CachingBehavior_Should_CacheHandlerValue_OnMissAsync()
    {
        var cache = new Mock<ICacheService>();
        cache.Setup(x => x.GetAsync<CacheResponse>("payment:1", CancellationToken.None)).ReturnsAsync((CacheResponse?)null);
        var behavior = new CachingBehavior<CacheableRequest, CacheResponse>(cache.Object);
        var expected = new CacheResponse("fresh");

        var result = await behavior.Handle(
            new CacheableRequest("payment:1"),
            _ => Task.FromResult(expected),
            CancellationToken.None);

        result.Should().BeSameAs(expected);
        cache.Verify(x => x.SetAsync("payment:1", expected, TimeSpan.FromMinutes(20), CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task CachingBehavior_Should_BypassCache_ForPlainRequestAsync()
    {
        var cache = new Mock<ICacheService>();
        var behavior = new CachingBehavior<PlainRequest, CacheResponse>(cache.Object);

        var result = await behavior.Handle(
            new PlainRequest(),
            _ => Task.FromResult(new CacheResponse("fresh")),
            CancellationToken.None);

        result.Value.Should().Be("fresh");
        cache.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task BusinessMemoryCache_Should_SetGetExpireAndRemoveByPrefixAsync()
    {
        var cache = new MemoryCacheService();
        await cache.SetAsync("payments:1", new CacheResponse("one"), TimeSpan.FromMinutes(1), CancellationToken.None);
        await cache.SetAsync("other:1", new CacheResponse("other"), TimeSpan.FromMinutes(1), CancellationToken.None);
        await cache.SetAsync("expired", new CacheResponse("old"), TimeSpan.FromMilliseconds(-1), CancellationToken.None);

        (await cache.GetAsync<CacheResponse>("payments:1", CancellationToken.None))!.Value.Should().Be("one");
        (await cache.GetAsync<CacheResponse>("missing", CancellationToken.None)).Should().BeNull();
        (await cache.GetAsync<CacheResponse>("expired", CancellationToken.None)).Should().BeNull();

        await cache.RemoveByPrefixAsync("payments:", CancellationToken.None);

        (await cache.GetAsync<CacheResponse>("payments:1", CancellationToken.None)).Should().BeNull();
        (await cache.GetAsync<CacheResponse>("other:1", CancellationToken.None)).Should().NotBeNull();
    }

    [Fact]
    public async Task InfrastructureMemoryCache_Should_SetGetAndRemoveByPrefixAsync()
    {
        using var memory = new MemoryCache(new MemoryCacheOptions { SizeLimit = 1_000 });
        var cache = new InfrastructureCache(memory);
        await cache.SetAsync("payments:1", new CacheResponse("one"), TimeSpan.FromMinutes(1), CancellationToken.None);
        await cache.SetAsync("other:1", new CacheResponse("other"), TimeSpan.FromMinutes(1), CancellationToken.None);

        (await cache.GetAsync<CacheResponse>("payments:1", CancellationToken.None))!.Value.Should().Be("one");

        await cache.RemoveByPrefixAsync("payments:", CancellationToken.None);

        (await cache.GetAsync<CacheResponse>("payments:1", CancellationToken.None)).Should().BeNull();
        (await cache.GetAsync<CacheResponse>("other:1", CancellationToken.None)).Should().NotBeNull();
    }

    [Theory]
    [InlineData(PaymentMethod.CreditCard, Currency.EGB, 102.5)]
    [InlineData(PaymentMethod.PayPal, Currency.USD, 100.09)]
    [InlineData(PaymentMethod.MobileWallets, Currency.EUR, 100.02586206896551724137931034)]
    public void PaymentMethods_Should_ApplyCommissionUsingCurrencyRate(
        PaymentMethod method,
        Currency currency,
        decimal expected)
    {
        var service = method switch
        {
            PaymentMethod.CreditCard => (TransitNovaPayment.Busieness.Common.Abstract.Abstraction.PaymentMethodService)new CreditCard(),
            PaymentMethod.PayPal => new PaypalPayment(),
            PaymentMethod.MobileWallets => new WalletsPayment(),
            _ => throw new ArgumentOutOfRangeException(nameof(method))
        };

        service.PaymentMethod.Should().Be(method);
        service.Pay(100m, currency).Should().BeApproximately(expected, 0.000001m);
    }

    [Fact]
    public void PaymentMethod_Should_RejectUnsupportedCurrency()
    {
        var service = new CreditCard();

        var act = () => service.Pay(100m, (Currency)999);

        act.Should().Throw<NotSupportedException>().WithMessage("*999*");
    }

    [Fact]
    public async Task CacheImplementations_Should_HonorCancellationAsync()
    {
        using var source = new CancellationTokenSource();
        source.Cancel();
        var businessCache = new MemoryCacheService();
        using var memory = new MemoryCache(new MemoryCacheOptions { SizeLimit = 1_000 });
        var infrastructureCache = new InfrastructureCache(memory);

        await FluentActions.Awaiting(() => businessCache.GetAsync<CacheResponse>("key", source.Token)).Should().ThrowAsync<OperationCanceledException>();
        await FluentActions.Awaiting(() => infrastructureCache.SetAsync("key", new CacheResponse("value"), TimeSpan.FromMinutes(1), source.Token)).Should().ThrowAsync<OperationCanceledException>();
    }

    private sealed record TestCommand(string Name) : ICommand<BaseResult>;
    private sealed record TestQuery(string Name) : IRequest<BaseResult>;
    private sealed record CacheableRequest(string CacheKey) : IRequest<CacheResponse>, ICachable;
    private sealed record PlainRequest : IRequest<CacheResponse>;
    private sealed record CacheResponse(string Value);
}


