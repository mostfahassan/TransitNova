
using Bogus;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using TransitNova.BusinessLayer.DTOs.Reports;
using TransitNova.Domain.Contracts.Constants;
using TransitNova.Domain.Contracts.DomainEvents;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Entities.MainEntities;
using TransitNova.Domain.Enums.Bundle;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Payment;
using TransitNova.Domain.Enums.Shipment;
using TransitNova.Domain.Enums.SystemLogs;
using TransitNova.Domain.Enums.Trip;
using TransitNova.Domain.Enums.Users;
using TransitNova.Domain.Enums.Warehouse;

namespace TransitNova.InfraStructure.Context.Seeder
{
    public static class DatabaseSeeder
    {
        private const string SeedEmailDomain = "seed.transitnova.local";
        private const string DemoPassword = "TransitNova@12345";
        private static readonly DateTime SeedClock = new(2026, 7, 1, 8, 0, 0, DateTimeKind.Utc);

        private static readonly string[] LocationScriptNames = ["Countries.sql", "Governments.sql", "Cities.sql"];

        public static async Task SeedLocationLookupDataAsync(IServiceProvider services, ILogger logger, CancellationToken ct = default)
        {
            var context = services.GetRequiredService<AppDbContext>();

            var hasCountries = await context.Countries.AnyAsync(ct);
            var hasGovernments = await context.Governments.AnyAsync(ct);
            var hasCities = await context.Cities.AnyAsync(ct);

            if (hasCountries || hasGovernments || hasCities)
            {
                if (hasCountries && hasGovernments && hasCities)
                {
                    logger.LogInformation("Location lookup seed already exists. Skipping Countries, Governments, and Cities.");
                    return;
                }

                throw new InvalidOperationException("Location lookup tables are partially populated. Clear Countries, Governments, and Cities or complete the location seed before rerunning startup seeding.");
            }

            var executionStrategy = context.Database.CreateExecutionStrategy();
            await executionStrategy.ExecuteAsync(async () =>
            {
                await using var tx = await context.Database.BeginTransactionAsync(ct);

                foreach (var scriptName in LocationScriptNames)
                {
                    var sql = LoadLocationScript(scriptName);
                    if (string.IsNullOrWhiteSpace(sql))
                        continue;

                    await context.Database.ExecuteSqlRawAsync(sql, ct);
                }

                await tx.CommitAsync(ct);
            });

            var countryCount = await context.Countries.CountAsync(ct);
            var governmentCount = await context.Governments.CountAsync(ct);
            var cityCount = await context.Cities.CountAsync(ct);

            logger.LogInformation(
                "Seeded location lookup data: {Countries} countries, {Governments} governments, {Cities} cities.",
                countryCount,
                governmentCount,
                cityCount);
        }

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
                throw new InvalidOperationException("DatabaseSeeder requires location lookup data before demo seeding. Add Countries, Governments, and Cities to the database, then rerun with SeedDemoData=true.");

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
            context.PaymentInvoices.AddRange(seed.PaymentInvoices);
            context.Notifications.AddRange(seed.Notifications);
            context.ReportRequests.AddRange(seed.ReportRequests);
            context.SystemActivityLogs.AddRange(seed.SystemActivityLogs);

            await context.SaveChangesAsync(ct);
            await tx.CommitAsync(ct);

            logger.LogInformation("Seeded TransitNova demo data: {Customers} customers, {Carriers} carriers, {Vehicles} vehicles, {Warehouses} warehouses, {Shipments} shipments, {Trips} trips, {Invoices} invoices, {Notifications} notifications, {Reports} reports, {ActivityLogs} activity logs. Demo password for all seeded user types is {DemoPassword}.", seed.UserProfiles.Count, seed.Carriers.Count, seed.Vehicles.Count, seed.Warehouses.Count, seed.Shipments.Count, seed.Trips.Count, seed.PaymentInvoices.Count, seed.Notifications.Count, seed.ReportRequests.Count, seed.SystemActivityLogs.Count, DemoPassword);
        }

        private static string LoadLocationScript(string scriptName)
        {
            var assembly = typeof(DatabaseSeeder).Assembly;
            var resourceName = assembly
                .GetManifestResourceNames()
                .SingleOrDefault(name => name.EndsWith($".LocationScripts.{scriptName}", StringComparison.OrdinalIgnoreCase));

            if (resourceName is null)
                throw new InvalidOperationException($"Embedded location script '{scriptName}' was not found.");

            using var stream = assembly.GetManifestResourceStream(resourceName)
                ?? throw new InvalidOperationException($"Embedded location script '{resourceName}' could not be opened.");
            using var reader = new StreamReader(stream);

            var script = reader.ReadToEnd();
            script = Regex.Replace(script, @"^\s*USE\s+\[[^\]]+\]\s*$", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);
            script = Regex.Replace(script, @"^\s*GO\s*$", string.Empty, RegexOptions.Multiline | RegexOptions.IgnoreCase);

            return script.Trim();
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
            List<CarrierRating> CarrierRatings,
            List<PaymentInvoice> PaymentInvoices,
            List<Notification> Notifications,
            List<ReportRequest> ReportRequests,
            List<SystemActivityLog> SystemActivityLogs);
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
                    Reflect.Set(carrier, nameof(Carrier.HandlerId), operationManagers[i % operationManagers.Count].Id);
                    foreach (var zone in Pick(zones, i * 2, 6)) carrier.ServedZones.Add(zone);
                    return carrier;
                }).ToList();

                var vehicles = carriers.Select((c, i) => _vehicles.Create(c, i)).ToList();
                var bundles = CreateBundles(admins[0].Id);
                foreach (var pair in userProfiles.Take(15).Select((u, i) => (u, i)))
                {
                    var bundle = bundles[pair.i % bundles.Count];
                    bundle.Subscribe(pair.u.Id);
                    var subscription = bundle.Subscriptions.Single(s => s.SubscribedUserId == pair.u.Id);
                    subscription.SubscriptionDate = SeedClock.AddDays(-(pair.i % 30));
                    subscription.EndDate = subscription.SubscriptionDate.AddMonths(bundle.BundleDurationMonths);
                }

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
                var paymentInvoices = BuildPaymentInvoices(shipments, bundles);
                var notifications = BuildNotifications(userProfiles, carriers, operationManagers, warehouseManagers, admins, shipments);
                var reportRequests = BuildReportRequests(userProfiles, admins, paymentInvoices, shipments);
                var systemActivityLogs = BuildSystemActivityLogs(admins, operationManagers, carriers, shipments, trips, warehouses);

                foreach (var aggregate in userProfiles.Cast<IAggregateRoot>()
                             .Concat(warehouseManagers)
                             .Concat(operationManagers)
                             .Concat(admins)
                             .Concat(carriers)
                             .Concat(bundles)
                             .Concat(shipments)
                             .Concat(trips)
                             .Concat(notifications)
                             .Concat(reportRequests))
                    aggregate.ClearDomainEvents();

                return new SeedData(users, userRoles, userProfiles, receivers, admins, operationManagers, warehouseManagers, zones, warehouses, carriers, vehicles, bundles, shipments, trips, carrierRatings, paymentInvoices, notifications, reportRequests, systemActivityLogs);
            }

            private List<AppUser> CreateUsers(UserType type, int count, string prefix, List<AppUser> users, List<IdentityUserRole<Guid>> roles)
            {
                var created = Enumerable.Range(1, count).Select(i => _accounts.Create(type, i, prefix)).ToList();
                users.AddRange(created);
                if (_roles.TryGetValue(type.ToString(), out var roleId)) roles.AddRange(created.Select(u => new IdentityUserRole<Guid> { UserId = u.Id, RoleId = roleId }));
                return created;
            }

            static List<Bundle> CreateBundles(Guid adminId)
            {
                var bundles = new List<Bundle>
                 {
                    Bundle.Create(adminId.ToString(), "Starter Shipping Bundle", "25 standard shipments, city-to-city support, basic tracking, and email notifications.", 1499m, BundleTier.Standard, 1, 25, 250m, 1200m, 5m, 100m),
                    Bundle.Create(adminId.ToString(), "Growth Logistics Bundle", "80 mixed shipments, priority pickup windows, warehouse handoff support, and operational reporting.", 3999m, BundleTier.Pro, 3, 80, 950m, 4500m, 10m, 500m),
                    Bundle.Create(adminId.ToString(), "Enterprise Express Bundle", "220 express/fragile shipments, dedicated review, premium support, analytics, and SLA monitoring.", 8999m, BundleTier.Plus, 6, 220, 3200m, 15000m, 15m, 1000m)
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


            private static List<PaymentInvoice> BuildPaymentInvoices(IReadOnlyList<Shipment> shipments, IReadOnlyList<Bundle> bundles)
            {
                var invoices = new List<PaymentInvoice>();
                var activeSubscriptions = new Dictionary<Guid, (Bundle Bundle, BundleSubscription Subscription)>();
                foreach (var bundle in bundles)
                {
                    foreach (var subscription in bundle.Subscriptions.Where(s => s.IsActive))
                        activeSubscriptions[subscription.SubscribedUserId] = (bundle, subscription);
                }

                var appliedBenefitCounts = new Dictionary<(Guid UserId, Guid BundleId), int>();

                foreach (var pair in shipments.Select((shipment, index) => (shipment, index)))
                {
                    var shipment = pair.shipment;
                    var paymentId = Reflect.Guid("shipment-payment", pair.index);
                    shipment.SetPaymentId(paymentId);

                    var originalCost = shipment.ShipmentCost;
                    var finalCost = originalCost;
                    decimal discountPercentage = 0;
                    decimal discountAmount = 0;
                    Guid? bundleSubscriptionId = null;
                    Guid? bundleId = null;
                    string? bundleName = null;
                    var benefitApplied = false;

                    if (activeSubscriptions.TryGetValue(shipment.SenderId, out var seedSubscription))
                    {
                        var bundle = seedSubscription.Bundle;
                        var usageKey = (shipment.SenderId, bundle.Id);
                        appliedBenefitCounts.TryGetValue(usageKey, out var appliedCount);
                        var withinMonthlyLimit = bundle.MaxShipmentsPerMonth <= 0 || appliedCount < bundle.MaxShipmentsPerMonth;
                        var withinWeightLimit = bundle.MaxWeightPerShipment <= 0 || shipment.PackageSpecification.Weight <= bundle.MaxWeightPerShipment;
                        var aboveMinimumValue = bundle.MinimumShipmentValueForDiscount <= 0 || originalCost >= bundle.MinimumShipmentValueForDiscount;

                        if (withinMonthlyLimit && withinWeightLimit && aboveMinimumValue && bundle.DiscountPercentage > 0)
                        {
                            discountPercentage = bundle.DiscountPercentage;
                            discountAmount = Math.Round(originalCost * (discountPercentage / 100m), 2, MidpointRounding.AwayFromZero);
                            finalCost = Math.Max(0m, originalCost - discountAmount);
                            bundleSubscriptionId = seedSubscription.Subscription.Id;
                            bundleId = bundle.Id;
                            bundleName = bundle.BundleName;
                            benefitApplied = discountAmount > 0;
                            appliedBenefitCounts[usageKey] = appliedCount + 1;
                        }
                    }

                    var commission = Math.Round(finalCost * 0.08m, 2, MidpointRounding.AwayFromZero);
                    var invoice = PaymentInvoice.Create(
                        paymentId,
                        shipment.Id,
                        shipment.SenderId,
                        finalCost,
                        commission,
                        finalCost + commission,
                        shipment.PaymentMethod,
                        PaymentStatus.Success,
                        Constant.PaymentReferenceConstants.Shipment,
                        shipment.CreatedAt.AddMinutes(2),
                        benefitApplied ? $"{bundleName} discount applied to seeded shipment." : "Seeded successful shipment payment.",
                        bundleSubscriptionId,
                        bundleId,
                        bundleName,
                        originalCost,
                        discountPercentage,
                        discountAmount,
                        finalCost,
                        benefitApplied);

                    Reflect.Set(invoice, nameof(PaymentInvoice.Id), Reflect.Guid("shipment-invoice", pair.index));
                    Reflect.Dates(invoice, shipment.CreatedAt.AddMinutes(2), shipment.SenderId);
                    invoices.Add(invoice);
                }

                var bundleInvoiceIndex = 0;
                foreach (var bundle in bundles)
                {
                    foreach (var subscription in bundle.Subscriptions.Where(s => s.IsActive))
                    {
                        var paymentId = Reflect.Guid("bundle-payment", bundleInvoiceIndex);
                        var commission = Math.Round(bundle.BundlePrice * 0.04m, 2, MidpointRounding.AwayFromZero);
                        var invoice = PaymentInvoice.Create(
                            paymentId,
                            bundle.Id,
                            subscription.SubscribedUserId,
                            bundle.BundlePrice,
                            commission,
                            bundle.BundlePrice + commission,
                            PaymentMethod.CreditCard,
                            PaymentStatus.Success,
                            Constant.PaymentReferenceConstants.Bundle,
                            subscription.SubscriptionDate.AddMinutes(3),
                            "Seeded successful bundle subscription payment.");

                        Reflect.Set(invoice, nameof(PaymentInvoice.Id), Reflect.Guid("bundle-invoice", bundleInvoiceIndex));
                        Reflect.Dates(invoice, subscription.SubscriptionDate.AddMinutes(3), subscription.SubscribedUserId);
                        invoices.Add(invoice);
                        bundleInvoiceIndex++;
                    }
                }

                return invoices;
            }

            private static List<Notification> BuildNotifications(
                IReadOnlyList<UserProfile> users,
                IReadOnlyList<Carrier> carriers,
                IReadOnlyList<OperationManagerProfile> operationManagers,
                IReadOnlyList<WarehouseManagerProfile> warehouseManagers,
                IReadOnlyList<AdminProfile> admins,
                IReadOnlyList<Shipment> shipments)
            {
                var notifications = new List<Notification>();

                foreach (var pair in shipments.Take(30).Select((shipment, index) => (shipment, index)))
                {
                    var notification = Notification.Create(
                        pair.shipment.Sender.AppUserId,
                        "Shipment status update",
                        $"Shipment {pair.shipment.TrackingNumber} is now {pair.shipment.CurrentStatus}.");
                    AddNotification(notifications, notification, pair.index, pair.shipment.CreatedAt.AddHours(2), markAsRead: pair.index % 3 == 0);
                }

                foreach (var pair in carriers.Take(10).Select((carrier, index) => (carrier, index)))
                {
                    var notification = Notification.Create(
                        pair.carrier.AppUserId,
                        "Carrier workload assigned",
                        "Your seeded roster includes active trips and shipment history for dashboard review.");
                    AddNotification(notifications, notification, 100 + pair.index, SeedClock.AddDays(-(pair.index % 10)).AddHours(4), markAsRead: pair.index % 2 == 0);
                }

                foreach (var pair in operationManagers.Select((manager, index) => (manager, index)))
                {
                    var notification = Notification.Create(
                        pair.manager.AppUserId,
                        "Operations queue ready",
                        "Demo shipments, carriers, and trips have been assigned to your operation manager scope.");
                    AddNotification(notifications, notification, 200 + pair.index, SeedClock.AddDays(-pair.index).AddHours(5), markAsRead: false);
                }

                foreach (var pair in warehouseManagers.Take(5).Select((manager, index) => (manager, index)))
                {
                    var notification = Notification.Create(
                        pair.manager.AppUserId,
                        "Warehouse board ready",
                        "Warehouse shipments and trips are available for the demo command board.");
                    AddNotification(notifications, notification, 300 + pair.index, SeedClock.AddDays(-pair.index).AddHours(6), markAsRead: pair.index % 2 == 1);
                }

                foreach (var pair in admins.Select((admin, index) => (admin, index)))
                {
                    var notification = Notification.Create(
                        pair.admin.AppUserId,
                        "Demo tenant seeded",
                        "TransitNova demo operational data is ready for admin review.");
                    AddNotification(notifications, notification, 400 + pair.index, SeedClock.AddHours(7 + pair.index), markAsRead: pair.index != 0);
                }

                return notifications;
            }

            private static void AddNotification(List<Notification> notifications, Notification notification, int index, DateTime createdOnUtc, bool markAsRead)
            {
                Reflect.Set(notification, nameof(Notification.Id), Reflect.Guid("notification", index));
                Reflect.Set(notification, nameof(Notification.CreatedOnUtc), createdOnUtc);
                if (markAsRead)
                    notification.MarkAsRead();
                notifications.Add(notification);
            }

            private static List<ReportRequest> BuildReportRequests(
                IReadOnlyList<UserProfile> users,
                IReadOnlyList<AdminProfile> admins,
                IReadOnlyList<PaymentInvoice> invoices,
                IReadOnlyList<Shipment> shipments)
            {
                var reports = new List<ReportRequest>();
                var adminUserId = admins[0].AppUserId;
                var userId = users[0].AppUserId;
                var shipmentInvoice = invoices.First(x => x.ReferecneType == Constant.PaymentReferenceConstants.Shipment);
                var bundleInvoice = invoices.FirstOrDefault(x => x.ReferecneType == Constant.PaymentReferenceConstants.Bundle);

                reports.Add(CreateCompletedReport(
                    DashboardReportContract.ReportKey,
                    JsonSerializer.Serialize(new DashboardReportContract()),
                    adminUserId,
                    "reports/demo/dashboard-summary.pdf",
                    214_528,
                    0));

                reports.Add(CreateCompletedReport(
                    ShipmentReportContract.ReportKey,
                    JsonSerializer.Serialize(new ShipmentReportContract { ShipmentId = shipments[0].Id }),
                    adminUserId,
                    "reports/demo/shipment-analysis.pdf",
                    178_240,
                    1));

                reports.Add(CreateCompletedReport(
                    InvoiceReportContract.ReportKey,
                    JsonSerializer.Serialize(new InvoiceReportContract { PaymentId = shipmentInvoice.PaymentId }),
                    userId,
                    "reports/demo/invoice.pdf",
                    96_512,
                    2));

                if (bundleInvoice is not null)
                {
                    reports.Add(CreateStartedReport(
                        BundleReportContract.ReportKey,
                        JsonSerializer.Serialize(new BundleReportContract { PaymentId = bundleInvoice.PaymentId }),
                        adminUserId,
                        3));
                }

                reports.Add(CreatePendingReport(
                    ShipmentReportContract.ReportKey,
                    JsonSerializer.Serialize(new ShipmentReportContract { ShipmentId = shipments[1].Id }),
                    userId,
                    4));

                return reports;
            }

            private static ReportRequest CreatePendingReport(string reportKey, string payloadJson, Guid requestedBy, int index)
            {
                var report = ReportRequest.CreateReport(reportKey, payloadJson, requestedBy);
                Reflect.Set(report, nameof(ReportRequest.Id), Reflect.Guid("report-request", index));
                Reflect.Dates(report, SeedClock.AddDays(-index), requestedBy);
                return report;
            }

            private static ReportRequest CreateStartedReport(string reportKey, string payloadJson, Guid requestedBy, int index)
            {
                var report = CreatePendingReport(reportKey, payloadJson, requestedBy, index);
                report.MarkAsStarted();
                Reflect.Set(report, nameof(ReportRequest.StartedAt), SeedClock.AddDays(-index).AddMinutes(10));
                return report;
            }

            private static ReportRequest CreateCompletedReport(string reportKey, string payloadJson, Guid requestedBy, string filePath, int fileSize, int index)
            {
                var report = CreateStartedReport(reportKey, payloadJson, requestedBy, index);
                report.MarkAsCompleted(filePath, fileSize);
                Reflect.Set(report, nameof(ReportRequest.CompletedAt), SeedClock.AddDays(-index).AddMinutes(25));
                return report;
            }

            private static List<SystemActivityLog> BuildSystemActivityLogs(
                IReadOnlyList<AdminProfile> admins,
                IReadOnlyList<OperationManagerProfile> operationManagers,
                IReadOnlyList<Carrier> carriers,
                IReadOnlyList<Shipment> shipments,
                IReadOnlyList<Trip> trips,
                IReadOnlyList<Warehouse> warehouses)
            {
                var logs = new List<SystemActivityLog>();
                AddActivity(logs, ActivityAction.Created, ActivityEntityType.Warehouse, $"Seeded {warehouses[0].Name}.", admins[0].AppUserId, admins[0].FullName, 0);
                AddActivity(logs, ActivityAction.Approved, ActivityEntityType.Shipment, $"Approved shipment {shipments[3].TrackingNumber}.", operationManagers[0].AppUserId, operationManagers[0].FullName, 1);
                AddActivity(logs, ActivityAction.Assigned, ActivityEntityType.Carrier, $"Assigned carrier {carriers[0].Code} to an operations handler.", operationManagers[0].AppUserId, operationManagers[0].FullName, 2);
                AddActivity(logs, ActivityAction.Started, ActivityEntityType.Trip, $"Started trip {trips.First().Id.ToString()[..8]}.", operationManagers[0].AppUserId, operationManagers[0].FullName, 3);
                AddActivity(logs, ActivityAction.Delivered, ActivityEntityType.Shipment, $"Delivered shipment {shipments.First(s => s.CurrentStatus == ShipmentStatuses.Delivered).TrackingNumber}.", carriers[0].AppUserId, carriers[0].FullName, 4);
                AddActivity(logs, ActivityAction.Updated, ActivityEntityType.User, "Demo users and role assignments seeded.", admins[0].AppUserId, admins[0].FullName, 5);
                return logs;
            }

            private static void AddActivity(List<SystemActivityLog> logs, ActivityAction action, ActivityEntityType entityType, string description, Guid performedByUserId, string performedByName, int index)
            {
                var log = SystemActivityLog.AddLog(action, entityType, description, performedByUserId, performedByName);
                Reflect.Set(log, nameof(SystemActivityLog.OccurredAt), SeedClock.AddDays(-index).AddHours(8));
                logs.Add(log);
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
            private Address Addr(City city) => Address.Create($"{city.Name} Logistics District", null, _faker.Address.StreetAddress());
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
                var warehouse = Warehouse.Create(
                        $"{city.Name} {(type == WarehouseType.MainWarehouse ? "Main Hub" : "Branch")}",
                        type,
                        Math.Round(capacity, 2),
                        Math.Round(capacity * _faker.Random.Decimal(0.35m, 0.88m), 2),
                        _faker.Random.Int(10, 24),
                        Address.Create(_faker.Address.StreetAddress(), "Cairo City", city.Name),
                        admin.Id,
                        manager.Id
                        );
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
                var carrier = Carrier.Create(user.Id, First(user), Last(user), user.Email!, user.PhoneNumber!, Address.Create($"{city.Name} Carrier Address", null, _faker.Address.StreetAddress()), city.Id);
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
                var receiver = ReceiverProfile.Create(_faker.Name.FirstName(), _faker.Name.LastName(), $"receiver.{index:00000}@{SeedEmailDomain}", Reflect.Phone(5000 + index), Address.Create($"{city.Name} Receiver Address", null, _faker.Address.StreetAddress()), city.Id, sender.Id);
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
                var shipment = Shipment.Create(sender.Id, receiver, package, Currency.EGP, SeedClock.AddDays(index % 21).AddHours(index % 9), Address.Create($"{receiverCity.Name} Delivery Point", null, _faker.Address.StreetAddress()), Address.Create($"{senderCity.Name} Pickup Point", null, _faker.Address.StreetAddress()), type, mode, (PaymentMethod)(index % 3));
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
