using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Tests.TestInfrastructure;

internal sealed class SqliteAppDbContextFixture : IAsyncDisposable
{
    private readonly SqliteConnection _connection;

    private SqliteAppDbContextFixture(SqliteConnection connection, TestAppDbContext context)
    {
        _connection = connection;
        Context = context;
    }

    public TestAppDbContext Context { get; }

    public static async Task<SqliteAppDbContextFixture> CreateAsync(
        params IInterceptor[] interceptors)
    {
        var connection = new SqliteConnection("Data Source=:memory:");
        await connection.OpenAsync();

        var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection);

        if (interceptors.Length > 0)
            optionsBuilder.AddInterceptors(interceptors);

        var context = new TestAppDbContext(optionsBuilder.Options);
        await context.Database.EnsureCreatedAsync();
        return new SqliteAppDbContextFixture(connection, context);
    }

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await _connection.DisposeAsync();
    }

    internal sealed class TestAppDbContext(DbContextOptions<AppDbContext> options)
        : AppDbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<IdempotentTable>().HasKey(x => x.RequestId);
          
            modelBuilder.Entity<Bundle>().Ignore(x => x.Subscriptions);
        }
    }
}
