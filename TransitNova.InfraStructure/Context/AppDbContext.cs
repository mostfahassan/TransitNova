
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.InfraStructure.OutBox;
namespace TransitNova.InfraStructure.Context
{
    public class AppDbContext : IdentityDbContext<AppUser, IdentityRole<Guid>, Guid>
    {
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<ReceiverProfile> ReceiverProfiles { get; set; }
        public DbSet<OperationManagerProfile> OperationManagerProfiles { get; set; }
        public DbSet<AdminProfile> Admins { get; set; }
        public DbSet<SystemActivityLog> SystemActivityLogs { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<OutboxMessage> OutboxMessages { get; set; }
        public DbSet<IdempotentTable> IdempotentTableKey { get; set; }
        public DbSet<Government> Governments { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Trip> Trips { get; set; }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<BundleSubscription> UserBundleSubscriptions { get; set; }
        public DbSet<Shipment> Shipments { get; set; }
        public DbSet<Payment> Payments { get; set; }
        public DbSet<PaymentHistory> PaymentHistories { get; set; }
        public DbSet<Carrier> Carriers { get; set; }
        public DbSet<CarrierRating> CarrierRatings { get; set; }
        public DbSet<Vehicle> Vehicles { get; set; }
   
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<ShipmentStatus> ShipmentStatuses { get; set; }
        public DbSet<Bundle> Bundles { get; set; }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.ConfigureWarnings(w =>
              w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
        }

        public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
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
