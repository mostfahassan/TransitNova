using FluentAssertions;
using TransitNova.Payment.Tests.TestInfrastructure;
using TransitNovaPayment.Busieness.Common.DTO.PaymentHistoryDto;
using TransitNovaPayment.Busieness.Models.PaymentEntity.PaymentEnums;
using TransitNovaPayment.Infrastructure.RepositoryImplementation.PaymentRepo;

namespace TransitNova.Payment.Tests.HistoryFiltering;

public sealed class PaymentHistoryFilteringTests
{
    [Fact]
    public async Task FilterPaymentHistoryAsync_ShouldApplyFiltersAndReturnPagedResultsAsync()
    {
        await using var fixture = await SqlitePaymentDbContextFixture.CreateAsync();
        var now = new DateTime(2026, 06, 27, 10, 0, 0, DateTimeKind.Utc);
        var creditPayment = PaymentTestData.CreatePayment(paymentMethod: PaymentMethod.CreditCard);
        var paypalPayment = PaymentTestData.CreatePayment(
            paymentMethod: PaymentMethod.PayPal,
            shipmentId: Guid.Parse("55555555-5555-5555-5555-555555555555"));
        var histories = new[]
        {
            PaymentTestData.CreateHistory(creditPayment, PaymentStatus.Pending, PaymentStatus.Success, now.AddDays(-2), "auditor-a"),
            PaymentTestData.CreateHistory(paypalPayment, PaymentStatus.Pending, PaymentStatus.Failed, now.AddDays(-1), "auditor-b"),
            PaymentTestData.CreateHistory(paypalPayment, PaymentStatus.Failed, PaymentStatus.Failed, now, "auditor-b")
        };

        fixture.Context.Payments.AddRange(creditPayment, paypalPayment);
        fixture.Context.PaymentHistory.AddRange(histories);
        await fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();
        var repository = new PaymentQueryRepository(fixture.Context);
        var filter = new FilterPaymentHistoryDto
        {
            PaymentMethod = PaymentMethod.PayPal,
            PaymentStatus = PaymentStatus.Failed,
            CreatedBy = "auditor-b",
            CreatedAtFrom = now.AddDays(-1).AddHours(-1),
            CreatedAtTo = now.AddHours(1),
            PageNumber = 1,
            PageSize = 1
        };

        var result = await repository.FilterPaymentHistoryAsync(filter, CancellationToken.None);

        result.TotalCount.Should().Be(2);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(1);
        result.Data.Should().ContainSingle();
        result.Data.Single().CreatedBy.Should().Be("auditor-b");
        result.Data.Single().NewStatus.Should().Be(nameof(PaymentStatus.Failed));
    }

    [Fact]
    public async Task FilterPaymentHistoryAsync_ShouldNormalizeInvalidPaginationValuesAsync()
    {
        await using var fixture = await SqlitePaymentDbContextFixture.CreateAsync();
        var payment = PaymentTestData.CreatePayment();
        fixture.Context.Payments.Add(payment);
        fixture.Context.PaymentHistory.Add(PaymentTestData.CreateHistory(
            payment,
            PaymentStatus.Pending,
            PaymentStatus.Success,
            new DateTime(2026, 06, 26, 0, 0, 0, DateTimeKind.Utc),
            "auditor"));
        await fixture.Context.SaveChangesAsync();
        fixture.Context.ChangeTracker.Clear();
        var repository = new PaymentQueryRepository(fixture.Context);

        var result = await repository.FilterPaymentHistoryAsync(
            new FilterPaymentHistoryDto { PageNumber = 0, PageSize = 0 },
            CancellationToken.None);

        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().Be(1);
    }
}
