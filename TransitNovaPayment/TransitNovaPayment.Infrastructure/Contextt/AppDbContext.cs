using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using TransitNovaPayment.Busieness.Models.PaymentEntity;
using TransitNovaPayment.Busieness.Models.PaymentHistoryEntity;
namespace TransitNovaPayment.Infrastructure.Contextt
{
    public class AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration) : DbContext(options)
    {
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentHistory > PaymentHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
            foreach (var property in modelBuilder.Model.GetEntityTypes()
            .SelectMany(t => t.GetProperties())
                        .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?)))
                       {
                           property.SetPrecision(18);
                           property.SetScale(2);
                       }           
        }
    }
}
