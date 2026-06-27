using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using TransitNova.Payment.Tests.TestInfrastructure;
using TransitNovaPayment.Infrastructure.RepositoryImplementation.PaymentRepo;

namespace TransitNova.Payment.Tests.Repositories;

public sealed class PaymentCommandRepositoryTests
{
    [Fact]
    public async Task CreatePaymentAsync_ShouldTrackPaymentWithoutSavingImplicitlyAsync()
    {
        await using var fixture = await SqlitePaymentDbContextFixture.CreateAsync();
        var repository = new PaymentCommandRepository(fixture.Context);
        var payment = PaymentTestData.CreatePayment();

        await repository.CreatePaymentAsync(payment, CancellationToken.None);

        fixture.Context.Entry(payment).State.Should().Be(EntityState.Added);
        (await fixture.Context.Payments.AsNoTracking().CountAsync()).Should().Be(0);
    }
}
