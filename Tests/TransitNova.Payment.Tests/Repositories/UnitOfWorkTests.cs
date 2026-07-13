using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TransitNova.Payment.Tests.TestInfrastructure;
using TransitNovaPayment.Infrastructure.RepositoryImplementation;

namespace TransitNova.Payment.Tests.Repositories;

public sealed class UnitOfWorkTests
{
    [Fact]
    public async Task SaveChangesAsync_Should_PersistPendingPaymentAsync()
    {
        await using var fixture = await SqlitePaymentDbContextFixture.CreateAsync();
        var sut = new UnitOfWork(fixture.Context);
        var payment = PaymentTestData.CreatePayment();
        fixture.Context.Payments.Add(payment);

        var affected = await sut.SaveChangesAsync(CancellationToken.None);

        affected.Should().Be(1);
        (await fixture.Context.Payments.AsNoTracking().SingleAsync()).Id.Should().Be(payment.Id);
    }

    [Fact]
    public async Task CommitAsync_Should_PersistTransactionAsync()
    {
        await using var fixture = await SqlitePaymentDbContextFixture.CreateAsync();
        var sut = new UnitOfWork(fixture.Context);
        await sut.BeginTransactionAsync(CancellationToken.None);
        fixture.Context.Payments.Add(PaymentTestData.CreatePayment());
        await sut.SaveChangesAsync(CancellationToken.None);

        await sut.CommitAsync(CancellationToken.None);

        (await fixture.Context.Payments.AsNoTracking().CountAsync()).Should().Be(1);
    }

    [Fact]
    public async Task RollbackAsync_Should_DiscardTransactionAsync()
    {
        await using var fixture = await SqlitePaymentDbContextFixture.CreateAsync();
        var sut = new UnitOfWork(fixture.Context);
        await sut.BeginTransactionAsync(CancellationToken.None);
        fixture.Context.Payments.Add(PaymentTestData.CreatePayment());
        await sut.SaveChangesAsync(CancellationToken.None);

        await sut.RollbackAsync(CancellationToken.None);
        fixture.Context.ChangeTracker.Clear();

        (await fixture.Context.Payments.AsNoTracking().CountAsync()).Should().Be(0);
    }

    [Fact]
    public async Task CommitAndRollback_Should_BeNoOp_WhenNoTransactionExistsAsync()
    {
        await using var fixture = await SqlitePaymentDbContextFixture.CreateAsync();
        var sut = new UnitOfWork(fixture.Context);

        var act = async () =>
        {
            await sut.CommitAsync(CancellationToken.None);
            await sut.RollbackAsync(CancellationToken.None);
        };

        await act.Should().NotThrowAsync();
    }
}
