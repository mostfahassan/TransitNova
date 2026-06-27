using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using PaymentEntity = TransitNovaPayment.Busieness.Models.PaymentEntity.Payment;
using TransitNovaPayment.Infrastructure.Context;

namespace TransitNova.Payment.Tests.TestInfrastructure;

internal sealed class SqlitePaymentDbContextFixture : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    private SqlitePaymentDbContextFixture(SqliteConnection connection, TestAppDbContext context)
    {
        _connection = connection;
        Context = context;
    }

    public TestAppDbContext Context { get; }

    public static async Task<SqlitePaymentDbContextFixture> CreateAsync()
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;
        var configuration = new ConfigurationBuilder().Build();
        var context = new TestAppDbContext(options, configuration);
        await context.Database.EnsureCreatedAsync();

        return new SqlitePaymentDbContextFixture(connection, context);
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
    }

    internal sealed class TestAppDbContext(
        DbContextOptions<AppDbContext> options,
        IConfiguration configuration) : AppDbContext(options, configuration)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<PaymentEntity>()
                .Property(payment => payment.RowVersion)
                .IsRequired(false);
        }
    }
}
