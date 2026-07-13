using Microsoft.Extensions.Options;
using TransitNovaPayment.Busieness.Common.Options;
using TransitNovaPayment.Busieness.Implementation;

namespace TransitNova.Payment.Tests.Behaviors;

public sealed class PaymentExecutionStrategyTests
{
    [Fact]
    public async Task ExecuteAsync_WhenSuccessIsForced_ShouldReturnSuccessfulResultWithoutDelayAsync()
    {
        var strategy = CreateStrategy(forcedSuccess: true, delayMilliseconds: 0);

        var result = await strategy.ExecuteAsync(CancellationToken.None);

        Assert.True(result.IsSuccess);
        Assert.Null(result.FailureReason);
    }

    [Fact]
    public async Task ExecuteAsync_WhenFailureIsForced_ShouldReturnDeterministicFailureAsync()
    {
        var strategy = CreateStrategy(forcedSuccess: false, delayMilliseconds: 0);

        var result = await strategy.ExecuteAsync(CancellationToken.None);

        Assert.False(result.IsSuccess);
        Assert.Equal("Insufficient funds in the account.", result.FailureReason);
    }

    [Fact]
    public async Task ExecuteAsync_WhenDelayIsCancelled_ShouldPropagateCancellationAsync()
    {
        var strategy = CreateStrategy(forcedSuccess: true, delayMilliseconds: 10_000);
        using var cancellation = new CancellationTokenSource();
        cancellation.Cancel();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => strategy.ExecuteAsync(cancellation.Token));
    }

    private static RandomizedPaymentExecutionStrategy CreateStrategy(bool? forcedSuccess, int delayMilliseconds)
        => new(Options.Create(new PaymentExecutionOptions
        {
            ForcedSuccess = forcedSuccess,
            DelayMilliseconds = delayMilliseconds
        }));
}
