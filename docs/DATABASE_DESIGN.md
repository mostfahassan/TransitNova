# Database Design Review

## Purpose

Document and evaluate the implemented relational model, relationships, indexes, concurrency, soft delete, migrations, and query-aligned database concerns.

## Scope

The main Identity and logistics `AppDbContext`, the Payment `AppDbContext`, EF Core configurations, model snapshots, migrations, repository access patterns, and startup migration behavior.

## Executive Summary

The main schema is well beyond a basic CRUD model. It combines ASP.NET Core Identity with operational entities, owned value objects, explicit foreign keys, optimistic concurrency, string enum storage, filtered uniqueness for active subscriptions, and extensive indexes on Shipment, Trip, Carrier, Vehicle, Invoice, and identity lookups. The separate Payment database preserves a clear service boundary.

The principal database gaps are missing indexes for notification and outbox access, inconsistent migration startup between Main and Payment APIs, automatic main-schema migration during application startup, convention-only configuration for operational queue tables, and a suppressed pending-model warning that can hide migration drift.

## Current Implementation

### Contexts and Migrations

| Context | Provider | Migration | Startup behavior |
| --- | --- | --- | --- |
| `TransitNova.InfraStructure.Context.AppDbContext` | SQL Server | `20260712073348_InitialMigration` | Main API checks and applies pending migrations except in Testing |
| `TransitNovaPayment.Infrastructure.Context.AppDbContext` | SQL Server | `20260712073648_InitialMigration` | Migration helper exists, but Payment `Program.cs` does not call it |

Both migrations were discovered successfully with `dotnet ef migrations list --no-connect`. The local EF tool is version 10.0.7 while runtime packages are 10.0.9; align patch versions before generating the next migration.

### Main Relationship Model

| Principal | Dependent or relation | Delete behavior and constraint |
| --- | --- | --- |
| `AppUser` | User, Admin, Carrier, Operation Manager, Warehouse Manager profiles | Unique `AppUserId`; profile configurations generally use Restrict |
| `UserProfile` | Sent Shipments, ReceiverProfiles, BundleSubscriptions, PaymentInvoices | Sender and customer ownership; destructive deletes are restricted where history must remain |
| `ReceiverProfile` | Received Shipments | Receiver deletion is restricted while shipments exist |
| `OperationManagerProfile` | Handled Shipments and Carriers | Shipment and carrier handler links use SetNull where configured |
| `Carrier` | Vehicles and Trips | Trip relation uses Cascade for physical carrier deletion; Carrier itself is soft-deleted |
| `Carrier` | Served Zones | Explicit many-to-many join with composite key |
| `Warehouse` | Zones | `WarehouseZones` join table with composite key |
| `Warehouse` | Trips | Trip-to-Warehouse uses Restrict |
| `Trip` | Shipments | Shipment `TripId` uses SetNull |
| `Shipment` | ShipmentStatus history | Status rows retain lifecycle history; command queries include history when required |
| `Bundle` | BundleSubscriptions | Restrict prevents deleting referenced bundle history |
| `BundleSubscription` | User and Bundle | Unique filtered index allows only one active user/bundle pair |
| `PaymentInvoice` | UserProfile | Restrict preserves invoice history |
| `RefreshToken` | AppUser | Cascade on user deletion |

PaymentInvoice stores `BundleId` and `BundleSubscriptionId` as audit identifiers without navigation foreign keys. That is a defensible snapshot design because invoice history can survive bundle changes or removal, but the absence of referential integrity must be explicit.

### Value and Enum Persistence

- Shipment package dimensions are owned columns.
- Shipment pickup and delivery addresses and Warehouse addresses use owned `Address` mappings.
- Decimal properties default to precision `(18,2)` in `OnModelCreating`; percentage is explicitly `(5,2)`.
- Shipment status, shipment type, transportation mode, currency, trip status/type, vehicle type, payment method/status, and warehouse type are stored as strings.
- String enum storage improves readability but enum member renames require a data migration.

### Optimistic Concurrency

`RowVersion` is configured for Shipment, Trip, Carrier, and Warehouse. This protects the operational aggregates most likely to receive concurrent updates. Handlers should consistently catch `DbUpdateConcurrencyException` and translate it to a 409 response; that translation is not centralized today.

### Soft Delete

`UnitOfWork.SaveChangesAsync` converts deleted `ISoftDeletable` entries into updates, sets `IsDeleted`, and records `DeletedOn`. Shipment and Carrier have query filters. Shipment configuration registers the same filter twice, which is harmless but redundant. Queries that intentionally need deleted records must use `IgnoreQueryFilters` explicitly and be authorization-protected.

## Engineering Analysis

### Index Strengths

- Shipment: unique TrackingNumber, status, sender, receiver, handler, trip, timestamps, and multi-column operational filters.
- Trip: status, carrier, warehouse, creation date, and status/carrier/warehouse composites.
- Carrier: unique Code and AppUserId, status, rating, handler, city, and update time.
- Vehicle: unique PlateNumber, carrier, active state, refrigeration, type, and carrier/active composite.
- Invoice: payment/reference/customer IDs, timestamps, status composites, and subscription-benefit audit query.
- Refresh token: unique token plus user and expiration access paths.
- Subscription: filtered unique active user/bundle index and EndDate.

### Missing Operational Indexes

`NotificationQueryRepository` filters by user, counts unread records, orders newest first, and pages. The current snapshot has no Notification indexes. Add a composite index aligned with `(UserId, IsRead, CreatedOnUtc)` and validate the actual SQL query plan.

`ProcessOutboxMessagesJob` filters unprocessed rows below the retry limit and orders by occurrence time. `OutboxMessages` has no corresponding index. Add a filtered SQL Server index for pending rows or a composite on `ProcessedOn`, `RetryCount`, and `OccuredAt`.

### Migration Governance

Main API migration-on-start is convenient for local Docker, but production replicas can race and the application identity needs schema-change privileges. Prefer a deployment job that applies migrations once, then starts least-privilege applications. Payment must follow the same policy.

`AppDbContext.OnConfiguring` ignores `PendingModelChangesWarning`. This prevents noisy failures but can conceal a real model/migration mismatch. Remove the suppression after migrations stabilize and let CI detect pending model changes.

The repository currently has one InitialMigration per database. If an older database has a prior migration history, replacing it with a new InitialMigration requires a documented reset or baseline procedure; it cannot be applied as a normal incremental update without reconciliation.

## Strengths

- Clear main and payment database boundary.
- Strong operational indexing on core logistics tables.
- Row-version concurrency on mutable aggregates.
- Owned value objects keep address and package consistency inside parent rows.
- Restrict and SetNull rules preserve historical records.
- Active subscription uniqueness is enforced in the database, not only application code.
- Invoice benefit fields form an immutable historical pricing snapshot.

## Weaknesses

- Notification and outbox tables lack query-aligned indexes.
- Payment startup does not invoke its migration helper.
- Pending model changes are suppressed.
- Some important entities rely on conventions rather than dedicated configuration classes.
- PaymentInvoice configuration duplicates PaymentMethod and Status mapping.
- Persisted spelling defects in `ReferecneId` and `ReferecneType` reduce schema quality.

## Risks

- Queue and notification scans grow with retained history.
- Payment deployment can start against an empty schema.
- Automatic startup migration can fail or race during rolling deployment.
- Physical Carrier deletion can cascade Trips despite normal soft-delete expectations.
- A future enum rename can make stored string values incompatible.
- Concurrent bundle benefit consumption can exceed a monthly quota unless eligibility and invoice creation share a properly isolated transaction and locking strategy.

## Trade-offs

String enums and audit snapshots prioritize readability and historical truth. They are suitable here, provided renames are migrated and snapshot IDs are not treated as guaranteed foreign keys. A single main database is also appropriate for the modular monolith; splitting each feature database would add transaction complexity without current scale evidence.

## Metrics

| Metric | Value |
| --- | ---: |
| Main domain entity files | 28 |
| Explicit main entity configurations | 23 |
| Main migrations | 1 |
| Payment migrations | 1 |
| Row-version aggregates | 4 |
| Database providers | SQL Server in production, SQLite in many repository tests |

## Production Readiness

The schema is strong for an MVP. Production migration governance, queue indexes, concurrency-response handling, and SQL Server integration tests remain required.

## Recommendations

### High Priority

1. Add Notification and Outbox indexes and verify execution plans.
2. Establish one deployment-time migration process for both contexts.
3. Add a SQL Server migration smoke test from an empty database.
4. Test and enforce bundle monthly-quota concurrency.

### Medium Priority

1. Remove pending-model warning suppression and fail CI when migrations are missing.
2. Centralize `DbUpdateConcurrencyException` handling as HTTP 409.
3. Add explicit configurations for Notification and Outbox retention, lengths, and indexes.
4. Review Carrier-to-Trip cascade behavior against soft-delete policy.

### Low Priority

1. Correct persisted spelling defects through explicit rename migrations.
2. Remove duplicate entity configuration calls.
3. Document enum data-migration rules.

## Future Improvements

Add partition and retention strategies for logs, notifications, idempotency, reports, and outbox history after production volume is known. Track slow queries through OpenTelemetry and SQL Server Query Store.

## Overall Score

**7.3/10**

## Final Verdict

The relational design is thoughtful and operationally aware. Its production blockers are migration governance and queue-path indexing rather than a need for schema redesign.
