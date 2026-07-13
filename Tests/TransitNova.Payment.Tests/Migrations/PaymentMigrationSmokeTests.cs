using FluentAssertions;
using Microsoft.EntityFrameworkCore.Migrations;
using TransitNova.Payment.Tests.TestInfrastructure;
using TransitNovaPayment.Infrastructure.Context;
using PaymentEntity = TransitNovaPayment.Busieness.Models.PaymentEntity.Payment;

namespace TransitNova.Payment.Tests.Migrations;

public sealed class PaymentMigrationSmokeTests
{
    [Fact]
    public void PaymentAssembly_Should_ContainInitialMigrationMetadata()
    {
        var migrations = typeof(AppDbContext).Assembly
            .GetTypes()
            .Where(type => typeof(Migration).IsAssignableFrom(type))
            .Select(type => type.GetCustomAttributes(typeof(MigrationAttribute), inherit: false)
                .OfType<MigrationAttribute>()
                .SingleOrDefault()?.Id)
            .Where(id => id is not null)
            .ToList();

        migrations.Should().Contain("20260712073648_InitialMigration");
    }

    [Fact]
    public async Task PaymentModel_Should_KeepConcurrencyAndAmountMetadataAsync()
    {
        await using var fixture = await SqlitePaymentDbContextFixture.CreateAsync();
        var entity = fixture.Context.Model.FindEntityType(typeof(PaymentEntity))
            ?? throw new InvalidOperationException("Payment entity is missing from the EF model.");

        entity.FindProperty(nameof(PaymentEntity.RowVersion)).Should().NotBeNull();
        entity.FindProperty(nameof(PaymentEntity.TotalAmount))!.GetPrecision().Should().Be(18);
        entity.FindProperty(nameof(PaymentEntity.TotalAmount))!.GetScale().Should().Be(2);
    }
}