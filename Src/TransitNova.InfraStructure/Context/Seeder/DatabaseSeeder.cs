
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using TransitNova.Domain.Contracts.DomainEvents;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.Trip;
using TransitNova.Domain.Enums.Users;
using TransitNova.Domain.Enums.Warehouse;
using TransitNova.InfraStructure.Context;

namespace TransitNova.InfraStructure.Context.Seeder
{
    public static class DatabaseSeeder
    {
        private const string SeedEmailDomain = "seed.transitnova.local";
        private const string DemoPassword = "TransitNova@12345";
        private static readonly DateTime SeedClock = new(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);

        public static async Task SeedDemoDataAsync(IServiceProvider services, ILogger logger, CancellationToken ct = default)
        {
            var context = services.GetRequiredService<AppDbContext>();

            if (await context.AppUsers.AnyAsync(u => u.Email != null && u.Email.EndsWith($"@{SeedEmailDomain}"), ct))
            {
                logger.LogInformation("TransitNova demo seed already exists. Skipping.");
                return;
            }

            var cities = await context.Cities.AsNoTracking().OrderBy(c => c.Id).ToListAsync(ct);
            if (cities.Count == 0)
                throw new InvalidOperationException("DatabaseSeeder requires existing Cities/Governments data. These lookup entities are intentionally not faked.");

            var roles = (await context.Roles.AsNoTracking().ToListAsync(ct))
                .Where(r => !string.IsNullOrWhiteSpace(r.Name))
                .ToDictionary(r => r.Name!, r => r.Id, StringComparer.OrdinalIgnoreCase);

            Randomizer.Seed = new Random(8675309);
            var seed = new SeedBuilder(cities, roles).Build();

            await using var tx = await context.Database.BeginTransactionAsync(ct);
            context.AppUsers.AddRange(seed.Users);
            context.UserRoles.AddRange(seed.UserRoles);
            context.UserProfiles.AddRange(seed.UserProfiles);
            context.ReceiverProfiles.AddRange(seed.Receivers);
            context.Admins.AddRange(seed.Admins);
            context.OperationManagerProfiles.AddRange(seed.OperationManagers);
            context.WarehouseManagersProfiles.AddRange(seed.WarehouseManagers);
            context.Zones.AddRange(seed.Zones);
            context.Warehouses.AddRange(seed.Warehouses);
            context.Carriers.AddRange(seed.Carriers);
            context.Vehicles.AddRange(seed.Vehicles);
            context.Bundles.AddRange(seed.Bundles);
            context.Shipments.AddRange(seed.Shipments);
            context.Trips.AddRange(seed.Trips);
            context.CarrierRatings.AddRange(seed.CarrierRatings);

            await context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            logger.LogInformation("Seeded TransitNova demo data: {Customers} customers, {Carriers} carriers, {Vehicles} vehicles, {Warehouses} warehouses, {Shipments} shipments, {Trips} trips.", seed.UserProfiles.Count, seed.Carriers.Count, seed.Vehicles.Count, seed.Warehouses.Count, seed.Shipments.Count, seed.Trips.Count);
        }

        private sealed record SeedData(
            List<AppUser> Users,
            List<IdentityUserRole<Guid>> UserRoles,
            List<UserProfile> UserProfiles,
            List<ReceiverProfile> Receivers,
            List<AdminProfile> Admins,
            List<OperationManagerProfile> OperationManagers,
            List<WarehouseManagerProfile> WarehouseManagers,
            List<Zone> Zones,
            List<Warehouse> Warehouses,
            List<Carrier> Carriers,
            List<Vehicle> Vehicles,
            List<Bundle> Bundles,
            List<Shipment> Shipments,
            List<Trip> Trips,
            List<CarrierRating> CarrierRatings);
        private sealed class SeedBuilder
        {
            private readonly IReadOnlyList<City> _cities;
            private readonly IReadOnlyDictionary<string, Guid> _roles;
            private readonly AccountFaker _accounts = new();
            private readonly ProfileFaker _profiles = new();
            private readonly ZoneFaker _zones = new();
            private readonly WarehouseFaker _warehouses = new();
            private readonly CarrierFaker _carriers = new();
            private readonly VehicleFaker _vehicles = new();
            private readonly ReceiverFaker _receivers = new();
            private readonly ShipmentFaker _shipments = new();
            private readonly RatingFaker _ratings = new();
            private int _shipmentIndex;

            public SeedBuilder(IReadOnlyList<City> cities, IReadOnlyDictionary<string, Guid> roles)
            {
                _cities = cities;
                _roles = roles;
            }

            public SeedData Build()
            {
                var users = new List<AppUser>();
                var userRoles = new List<IdentityUserRole<Guid>>();
                var customers = CreateUsers(UserType.User, 100, "customer", users, userRoles);
                var carrierUsers = CreateUsers(UserType.Carrier, 30, "carrier", users, userRoles);
                var wmUsers = CreateUsers(UserType.WarehouseManager, 20, "warehouse.manager", users, userRoles);
                var opUsers = CreateUsers(UserType.OperationManager, 5, "operation.manager", users, userRoles);
                var adminUsers = CreateUsers(UserType.Admin, 4, "admin", users, userRoles);

                var userProfiles = customers.Select((u, i) => _profiles.User(u, City(i), i)).ToList();
                var warehouseManagers = wmUsers.Select((u, i) => _profiles.WarehouseManager(u, City(i + 200), i)).ToList();
                var operationManagers = opUsers.Select((u, i) => _profiles.OperationManager(u, City(i + 240), i)).ToList();
                var admins = adminUsers.Select((u, i) => _profiles.Admin(u, City(i + 260), i)).ToList();
                var zones = _cities.Select((c, i) => _zones.Create(c, i)).ToList();

                var warehouses = warehouseManagers.Select((m, i) =>
                {
                    var warehouse = _warehouses.Create(m, admins[i % admins.Count], City(i + 300), i);
                    foreach (var zone in Pick(zones, i, 5)) warehouse.AddZone(zone);
                    return warehouse;
                }).ToList();

                var carriers = carrierUsers.Select((u, i) =>
                {
                    var carrier = _carriers.Create(u, City(i + 350), warehouses[i % warehouses.Count], i);
                    foreach (var zone in Pick(zones, i * 2, 6)) carrier.ServedZones.Add(zone);
                    return carrier;
                }).ToList();

                var vehicles = carriers.Select((c, i) => _vehicles.Create(c, i)).ToList();
                var bundles = CreateBundles(admins[0].Id);
                foreach (var pair in userProfiles.Take(15).Select((u, i) => (u, i))) bundles[pair.i % bundles.Count].Subscribe(pair.u.Id);

                var receivers = new List<ReceiverProfile>();
                var shipments = new List<Shipment>();
                var pickupCandidates = new List<Shipment>();

                foreach (var pair in userProfiles.Select((u, i) => (u, i)))
                {
                    var shipmentCount = 15 + pair.i % 6;
                    for (var offset = 0; offset < shipmentCount; offset++)
                    {
                        var index = _shipmentIndex++;
                        var receiverCity = City(index + 500);
                        var receiver = _receivers.Create(pair.u, receiverCity, index);
                        var bundle = pair.i < 15 ? bundles[pair.i % bundles.Count] : null;
                        var shipment = _shipments.Create(pair.u, receiver, City(pair.i), receiverCity, bundle, index);
                        ApplyInitialState(shipment, operationManagers[index % operationManagers.Count], carriers[index % carriers.Count], index);

                        if (shipment.CurrentStatus == ShipmentStatuses.AssignedToPickUpCarrier) pickupCandidates.Add(shipment);
                        receivers.Add(receiver);
                        shipments.Add(shipment);
                    }
                }

                var trips = BuildTrips(pickupCandidates, carriers, warehouses, operationManagers);
                var carrierRatings = BuildRatings(shipments, carriers, userProfiles);

                foreach (var aggregate in userProfiles.Cast<IAggregateRoot>().Concat(warehouseManagers).Concat(operationManagers).Concat(admins).Concat(carriers).Concat(bundles).Concat(shipments).Concat(trips))
                    aggregate.ClearDomainEvents();

                return new SeedData(users, userRoles, userProfiles, receivers, admins, operationManagers, warehouseManagers, zones, warehouses, carriers, vehicles, bundles, shipments, trips, carrierRatings);
            }

            private List<AppUser> CreateUsers(UserType type, int count, string prefix, List<AppUser> users, List<IdentityUserRole<Guid>> roles)
            {
                var created = Enumerable.Range(1, count).Select(i => _accounts.Create(type, i, prefix)).ToList();
                users.AddRange(created);
                if (_roles.TryGetValue(type.ToString(), out var roleId)) roles.AddRange(created.Select(u => new IdentityUserRole<Guid> { UserId = u.Id, RoleId = roleId }));
                return created;
            }

            private List<Bundle> CreateBundles(Guid adminId)
            {
                var bundles = new List<Bundle>
                {
                    Bundle.Create(adminId.ToString(), "Starter Shipping Bundle", 1499m, "25 standard shipments, city-to-city support, basic tracking, and email notifications.", 250m, 1200m, 25),
                    Bundle.Create(adminId.ToString(), "Growth Logistics Bundle", 3999m, "80 mixed shipments, priority pickup windows, warehouse handoff support, and operational reporting.", 950m, 4500m, 80),
                    Bundle.Create(adminId.ToString(), "Enterprise Express Bundle", 8999m, "220 express/fragile shipments, dedicated review, premium support, analytics, and SLA monitoring.", 3200m, 15000m, 220)
                };

                for (var i = 0; i < bundles.Count; i++)
                {
                    Reflect.Set(bundles[i], nameof(Bundle.Id), Reflect.Guid("bundle", i));
                    Reflect.Dates(bundles[i], SeedClock.AddDays(-90 + i), adminId);
                }

                return bundles;
            }
            private void ApplyInitialState(Shipment shipment, OperationManagerProfile handler, Carrier carrier, int index)
            {
                switch (index % 10)
                {
                    case 0: return;
                    case 1:
                        shipment.RejectShipment(handler.Id, "Address verification failed during operations review.");
                        return;
                    case 2:
                        shipment.CancelShipment();
                        return;
                    case 3:
                    case 4:
                        shipment.ApproveShipment(handler.Id);
                        return;
                    default:
                        shipment.ApproveShipment(handler.Id);
                        shipment.AssignToCarrier(ShipmentStatuses.AssignedToPickUpCarrier, carrier.Id, handler.Id);
                        return;
                }
            }

            private List<Trip> BuildTrips(List<Shipment> pickupCandidates, IReadOnlyList<Carrier> carriers, IReadOnlyList<Warehouse> warehouses, IReadOnlyList<OperationManagerProfile> handlers)
            {
                var trips = new List<Trip>();
                var warehouseReady = new List<Shipment>();
                var cursor = 0;

                for (var i = 0; i < 12; i++)
                {
                    var group = pickupCandidates.Skip(cursor).Take(6).ToList();
                    cursor += group.Count;
                    if (group.Count == 0) break;

                    var carrier = carriers[i % carriers.Count];
                    var handler = handlers[i % handlers.Count];
                    var trip = CreateTrip(carrier, warehouses[i % warehouses.Count], TripType.Pickup, group, handler, i);

                    if (i is 0 or 1) { trips.Add(trip); continue; }
                    if (i is 10 or 11) { trip.Cancel(handler.Id); trips.Add(trip); continue; }

                    trip.StartTrip(handler.Id, TripType.Pickup);
                    foreach (var shipment in group) shipment.AssignedAsPickupTrip(trip.Id, carrier.Id);

                    if (i is >= 2 and <= 7)
                    {
                        foreach (var shipment in group)
                        {
                            shipment.PickedUp(carrier.Id);
                            shipment.DeliveredToWarehouse(carrier.Id);
                            warehouseReady.Add(shipment);
                        }
                        trip.Complete(carrier.Id);
                    }
                    else
                    {
                        foreach (var shipment in group.Take(3)) shipment.PickedUp(carrier.Id);
                    }

                    trips.Add(trip);
                }

                var deliveryCandidates = pickupCandidates.Skip(cursor).Take(40).ToList();
                foreach (var pair in deliveryCandidates.Select((s, i) => (s, i)))
                {
                    var carrier = carriers[(pair.i + 20) % carriers.Count];
                    pair.s.AssignedAsPickupTrip(Reflect.Guid("bootstrap-pickup-trip", pair.i), carrier.Id);
                    pair.s.PickedUp(carrier.Id);
                    pair.s.DeliveredToWarehouse(carrier.Id);
                    Reflect.Set(pair.s, nameof(Shipment.TripId), null);
                }

                cursor = 0;
                for (var i = 0; i < 8; i++)
                {
                    var source = deliveryCandidates.Skip(cursor).Take(4).ToList();
                    cursor += source.Count;
                    if (source.Count == 0) break;

                    var carrier = carriers[(i + 12) % carriers.Count];
                    var handler = handlers[i % handlers.Count];
                    var trip = CreateTrip(carrier, warehouses[(i + 3) % warehouses.Count], TripType.Delivery, source, handler, i + 12);

                    if (i == 0) { trips.Add(trip); continue; }
                    if (i == 7) { trip.Cancel(handler.Id); trips.Add(trip); continue; }

                    trip.StartTrip(handler.Id, TripType.Delivery);
                    foreach (var shipment in source) shipment.AssignedAsDeliveryTrip(trip.Id, carrier.Id);

                    if (i is >= 1 and <= 4)
                    {
                        foreach (var shipment in source) shipment.Delivered(carrier.Id);
                        trip.Complete(carrier.Id);
                    }
                    else
                    {
                        foreach (var shipment in source.Take(2)) shipment.Delivered(carrier.Id);
                    }

                    trips.Add(trip);
                }

                return trips;
            }

            private Trip CreateTrip(Carrier carrier, Warehouse warehouse, TripType type, List<Shipment> shipments, OperationManagerProfile handler, int index)
            {
                var trip = Trip.Plan(carrier.Id, warehouse.Id, type, shipments);
                Reflect.Set(trip, nameof(Trip.Id), Reflect.Guid("trip", index));
                Reflect.Set(trip, nameof(Trip.PlannedDate), SeedClock.AddDays(index % 14).AddHours(2));
                Reflect.Dates(trip, SeedClock.AddDays(-(index % 20)), handler.Id);
                return trip;
            }

            private List<CarrierRating> BuildRatings(IReadOnlyList<Shipment> shipments, IReadOnlyList<Carrier> carriers, IReadOnlyList<UserProfile> users)
            {
                return shipments.Where(s => s.CurrentStatus == ShipmentStatuses.Delivered).Take(80).Select((s, i) =>
                {
                    var carrier = carriers[i % carriers.Count];
                    carrier.AddRating(4 + i % 2);
                    return _ratings.Create(carrier, s, users[i % users.Count], i);
                }).ToList();
            }

            private City City(int index) => _cities[index % _cities.Count];

            private static IEnumerable<Zone> Pick(IReadOnlyList<Zone> zones, int start, int count)
            {
                for (var i = 0; i < Math.Min(count, zones.Count); i++) yield return zones[(start + i) % zones.Count];
            }
        }
        private sealed class AccountFaker : Faker<AppUser>
        {
            private readonly Faker _faker = new("en");
            private readonly PasswordHasher<AppUser> _hasher = new();
            public AppUser Create(UserType type, int index, string prefix)
            {
                var first = _faker.Name.FirstName();
                var last = _faker.Name.LastName();
                var email = $"{prefix}.{index:000}@{SeedEmailDomain}".ToLowerInvariant();
                var user = new AppUser
                {
                    Id = Reflect.Guid($"app-user-{type}", index),
                    UserName = email,
                    NormalizedUserName = email.ToUpperInvariant(),
                    Email = email,
                    NormalizedEmail = email.ToUpperInvariant(),
                    EmailConfirmed = true,
                    PhoneNumber = Reflect.Phone(index + (int)type * 1000),
                    PhoneNumberConfirmed = true,
                    SecurityStamp = Reflect.Guid($"security-{type}", index).ToString("N"),
                    ConcurrencyStamp = Reflect.Guid($"concurrency-{type}", index).ToString("N"),
                    UserType = type,
                    FullName = $"{first} {last}"
                };

                user.PasswordHash = _hasher.HashPassword(user, DemoPassword);
                return user;
            }
        }

        private sealed class ProfileFaker : Faker<BaseInfo<Guid>>
        {
            private readonly Faker _faker = new("en");
            public UserProfile User(AppUser user, City city, int index)
            {
                var profile = UserProfile.Create(user.Id, First(user), Last(user), user.Email!, user.PhoneNumber!, Addr(city), city.Id);
                Reflect.Set(profile, nameof(UserProfile.Id), Reflect.Guid("user-profile", index));
                Reflect.Dates(profile, SeedClock.AddDays(-(index % 60)), user.Id);
                return profile;
            }

            public WarehouseManagerProfile WarehouseManager(AppUser user, City city, int index)
            {
                var profile = WarehouseManagerProfile.Create(user.Id, First(user), Last(user), user.Email!, user.PhoneNumber!, Addr(city), city.Id);
                Reflect.Set(profile, nameof(WarehouseManagerProfile.Id), Reflect.Guid("warehouse-manager", index));
                Reflect.Dates(profile, SeedClock.AddDays(-(index % 45)), user.Id);
                return profile;
            }

            public OperationManagerProfile OperationManager(AppUser user, City city, int index)
            {
                var profile = OperationManagerProfile.Create(user.Id, First(user), Last(user), user.Email!, user.PhoneNumber!, Addr(city), city.Id);
                Reflect.Set(profile, nameof(OperationManagerProfile.Id), Reflect.Guid("operation-manager", index));
                Reflect.Dates(profile, SeedClock.AddDays(-(index % 40)), user.Id);
                return profile;
            }

            public AdminProfile Admin(AppUser user, City city, int index)
            {
                var profile = AdminProfile.Create(user.Id, First(user), Last(user), user.Email!, user.PhoneNumber!, Addr(city), city.Id);
                Reflect.Set(profile, nameof(AdminProfile.Id), Reflect.Guid("admin", index));
                Reflect.Dates(profile, SeedClock.AddDays(-(index % 30)), user.Id);
                return profile;
            }

            private static string First(AppUser user) => (user.FullName ?? "Transit Nova").Split(' ', StringSplitOptions.RemoveEmptyEntries).First();
            private static string Last(AppUser user) => (user.FullName ?? "Transit Nova").Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).FirstOrDefault() ?? "Transit";
            private string Addr(City city) => $"{_faker.Address.StreetAddress()}, {city.Name}";
        }

        private sealed class ZoneFaker : Faker<Zone>
        {
            public Zone Create(City city, int index)
            {
                var zone = Zone.Create($"{city.Name} Logistics Zone", city.Id);
                Reflect.Set(zone, nameof(Zone.Id), Reflect.Guid("zone", city.Id));
                Reflect.Set(zone, nameof(Zone.Code), $"ZN-{city.Id:0000}");
                Reflect.Dates(zone, SeedClock.AddDays(-(index % 100)), null);
                return zone;
            }
        }

        private sealed class WarehouseFaker : Faker<Warehouse>
        {
            private readonly Faker _faker = new("en");
            public Warehouse Create(WarehouseManagerProfile manager, AdminProfile admin, City city, int index)
            {
                var type = index < 3 ? WarehouseType.MainWarehouse : WarehouseType.BranchWarehouse;
                var capacity = type == WarehouseType.MainWarehouse ? _faker.Random.Decimal(25000, 50000) : _faker.Random.Decimal(6000, 18000);
                var warehouse = Warehouse.Create($"{city.Name} {(type == WarehouseType.MainWarehouse ? "Main Hub" : "Branch")}", type, Math.Round(capacity, 2), Math.Round(capacity * _faker.Random.Decimal(0.35m, 0.88m), 2), _faker.Random.Int(10, 24), $"{_faker.Address.StreetAddress()}, {city.Name}", admin.Id, manager.Id);
                Reflect.Set(warehouse, nameof(Warehouse.Id), Reflect.Guid("warehouse", index));
                Reflect.Dates(warehouse, SeedClock.AddDays(-(index % 80)), admin.Id);
                return warehouse;
            }
        }

        private sealed class CarrierFaker : Faker<Carrier>
        {
            private readonly Faker _faker = new("en");
            public Carrier Create(AppUser user, City city, Warehouse warehouse, int index)
            {
                var carrier = Carrier.Create(user.Id, First(user), Last(user), user.Email!, user.PhoneNumber!, $"{_faker.Address.StreetAddress()}, {city.Name}", city.Id);
                Reflect.Set(carrier, nameof(Carrier.Id), Reflect.Guid("carrier", index));
                Reflect.Set(carrier, nameof(Carrier.Code), $"CR-SEED-{index:000}");
                Reflect.Dates(carrier, SeedClock.AddDays(-(index % 70)), user.Id);
                carrier.AddAdditionalData(user.Id, $"LIC-EG-{index:000000}", _faker.Random.Int(18, 42), Math.Round(_faker.Random.Decimal(8.5m, 24.75m), 2), _faker.Random.Int(1, 12), SeedClock.AddMonths(-(index % 24)), _faker.Random.Int(1, 4), warehouse.Id);
                return carrier;
            }

            private static string First(AppUser user) => (user.FullName ?? "Transit Nova").Split(' ', StringSplitOptions.RemoveEmptyEntries).First();
            private static string Last(AppUser user) => (user.FullName ?? "Transit Nova").Split(' ', StringSplitOptions.RemoveEmptyEntries).Skip(1).FirstOrDefault() ?? "Carrier";
        }
        private sealed class VehicleFaker : Faker<Vehicle>
        {
            private readonly Faker _faker = new("en");
            public Vehicle Create(Carrier carrier, int index)
            {
                var type = (VehicleType)((index % 3) + 1);
                var vehicle = Vehicle.Create(type, $"EG-{index + 1000:0000}-{_faker.Random.String2(2, "ABCDEFGHIJKLMNOPQRSTUVWXYZ")}", Math.Round(_faker.Random.Decimal(750, 5000), 2), Math.Round(_faker.Random.Decimal(8, 42), 2), type == VehicleType.RefrigeratedTruck || index % 9 == 0, carrier.Id);
                Reflect.Set(vehicle, nameof(Vehicle.Id), Reflect.Guid("vehicle", index));
                Reflect.Dates(vehicle, SeedClock.AddDays(-(index % 65)), carrier.Id);
                return vehicle;
            }
        }

        private sealed class ReceiverFaker : Faker<ReceiverProfile>
        {
            private readonly Faker _faker = new("en");
            public ReceiverProfile Create(UserProfile sender, City city, int index)
            {
                var receiver = ReceiverProfile.Create(_faker.Name.FirstName(), _faker.Name.LastName(), $"receiver.{index:00000}@{SeedEmailDomain}", Reflect.Phone(5000 + index), $"{_faker.Address.StreetAddress()}, {city.Name}", city.Id, sender.Id);
                Reflect.Set(receiver, nameof(ReceiverProfile.Id), Reflect.Guid("receiver", index));
                Reflect.Dates(receiver, SeedClock.AddDays(-(index % 90)), sender.Id);
                return receiver;
            }
        }

        private sealed class ShipmentFaker : Faker<Shipment>
        {
            private readonly Faker _faker = new("en");
            public Shipment Create(UserProfile sender, ReceiverProfile receiver, City senderCity, City receiverCity, Bundle? bundle, int index)
            {
                var type = index % 9 == 0 ? enShipmentType.Fragile : index % 4 == 0 ? enShipmentType.Express : enShipmentType.Standard;
                var mode = index % 13 == 0 ? TransportationMode.Air : TransportationMode.Land;
                var weight = type == enShipmentType.Fragile ? _faker.Random.Decimal(1.2m, 14m) : _faker.Random.Decimal(0.5m, 38m);
                var package = new PackageSpecification(Math.Round(weight, 2), Math.Round(_faker.Random.Decimal(15, 90), 2), Math.Round(_faker.Random.Decimal(10, 75), 2), Math.Round(_faker.Random.Decimal(10, 110), 2));
                var shipment = Shipment.Create(sender.Id, receiver, package, Currency.EGP, SeedClock.AddDays(index % 21).AddHours(index % 9), $"{_faker.Address.StreetAddress()}, {receiverCity.Name}", $"{_faker.Address.StreetAddress()}, {senderCity.Name}", type, mode, bundle?.Id, Reflect.Guid("payment", index), (PaymentMethod)(index % 3));
                var cost = package.Weight * (type == enShipmentType.Express ? 32m : 18m) + (type == enShipmentType.Fragile ? 85m : 0m) + _faker.Random.Decimal(25, 160);
                shipment.SetShipmentCost(Math.Round(cost, 2), SeedClock.AddDays(2 + index % 12));
                Reflect.Set(shipment, nameof(Shipment.Id), Reflect.Guid("shipment", index));
                Reflect.Set(shipment, nameof(Shipment.TrackingNumber), $"TRK-{index + 100000:000000}-{_faker.Random.String2(4, "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")}");
                Reflect.Dates(shipment, SeedClock.AddDays(-(index % 120)).AddMinutes(index % 60), sender.Id);
                return shipment;
            }
        }

        private sealed class RatingFaker : Faker<CarrierRating>
        {
            public CarrierRating Create(Carrier carrier, Shipment shipment, UserProfile user, int index)
            {
                var rating = CarrierRating.Create(carrier.Id, shipment.Id, user.Id, 4 + index % 2, index % 5 == 0 ? "Fast handoff and clear status updates." : "Reliable delivery experience.");
                Reflect.Set(rating, nameof(CarrierRating.Id), Reflect.Guid("carrier-rating", index));
                Reflect.Set(rating, nameof(CarrierRating.CreatedAt), SeedClock.AddDays(-(index % 20)));
                return rating;
            }
        }

        private static class Reflect
        {
            public static void Dates<T>(T entity, DateTime createdAt, Guid? createdBy) where T : class
            {
                Set(entity, nameof(BaseEntity<Guid>.CreatedAt), createdAt);
                if (createdBy.HasValue) Set(entity, nameof(BaseEntity<Guid>.CreatedBy), createdBy.Value.ToString());
            }

            public static void Set<T>(T entity, string name, object? value) where T : class
            {
                var type = entity.GetType();
                while (type is not null)
                {
                    var property = type.GetProperty(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                    if (property is not null)
                    {
                        property.SetValue(entity, value);
                        return;
                    }
                    type = type.BaseType;
                }
                throw new MissingMemberException(entity.GetType().Name, name);
            }

            public static Guid Guid(string scope, int index)
            {
                var bytes = SHA256.HashData(Encoding.UTF8.GetBytes($"TransitNova:{scope}:{index}"))[..16].ToArray();
                bytes[7] = (byte)((bytes[7] & 0x0F) | 0x40);
                bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);
                return new Guid(bytes);
            }

            public static string Phone(int index)
            {
                var operators = new[] { "10", "11", "12", "15" };
                return $"+20{operators[index % operators.Length]}{(10000000 + index % 90000000):00000000}";
            }
        }
    }
}



