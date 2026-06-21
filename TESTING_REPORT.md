# TransitNova Automated Testing Report

## Verification summary

Final command:

```text
dotnet test TransitNova.slnx --no-restore
```

| Project | Test classes | Tests | Passed | Failed | Skipped |
|---|---:|---:|---:|---:|---:|
| TransitNova.Domain.Tests | 13 | 115 | 115 | 0 | 0 |
| TransitNova.ApplicationLayer.Tests | 18 | 208 | 208 | 0 | 0 |
| TransitNova.InfraStructure.Tests | 4 | 38 | 38 | 0 | 0 |
| TransitNova.MappingTests | 1 | 2 | 2 | 0 | 0 |
| **Solution total** | **36** | **363** | **363** | **0** | **0** |

The requested Application and Infrastructure expansion added 110 executable cases: 72 Application cases and 38 Infrastructure cases. All tests are deterministic, have real assertions, and no test is skipped.

## Discovery inventory

| Component | Discovered |
|---|---:|
| Command-handler source files | 56 |
| Query-handler source files | 54 |
| FluentValidation validator source files | 81 |
| Pipeline behaviors | 4 |
| Repository implementation source files | 35 |
| EF Core entity configurations | 20 |
| Outbox writer | `ConvertDomainEventsToOutboxMessages` |
| Dedicated outbox processor | None |
| Background job / current processor | `ProcessOutboxMessagesJob` |

The application surface remains substantially larger than the executable test suite. The new suites prioritize shipment and carrier mutations, subscription state, the shipment read path, validation, all pipeline behavior categories, the outbox boundary, generic/idempotency/audit repositories, and persistence metadata.

## Files created in this expansion

### Application

| File | Executable cases |
|---|---:|
| `Commands/Shipments/ShipmentCommandHandlerTests.cs` | 10 |
| `Commands/OperationManager/ShipmentWorkflowCommandHandlerTests.cs` | 8 |
| `Commands/Carriers/CarrierCommandHandlerTests.cs` | 9 |
| `Commands/Bundles/BundleSubscriptionCommandHandlerTests.cs` | 8 |
| `Queries/Shipments/ShipmentQueryHandlerTests.cs` | 9 |
| `Validators/ShipmentDtoValidatorTests.cs` | 24 |
| **Created-file subtotal** | **68** |

`Behaviors/PipelineBehaviorTests.cs` was expanded by 4 cases, bringing the Application expansion to 72 cases. `TestData/ShipmentTestData.cs` is a test-data helper and contains no tests.

### Infrastructure

| File | Executable cases |
|---|---:|
| `Outbox/OutboxInterceptorTests.cs` | 6 |
| `BackgroundJobs/ProcessOutboxMessagesJobTests.cs` | 8 |
| `Repositories/RepositoryIntegrationTests.cs` | 15 |
| `EntityConfigurations/EntityConfigurationTests.cs` | 9 |
| **Created-file subtotal** | **38** |

`TestInfrastructure/SqliteAppDbContextFixture.cs` is the isolated SQLite fixture and contains no tests.

## Application coverage

The Application suite now reports 35.15% line coverage and 40.81% branch coverage for `TransitNova.BusinessLayer` (2,305 of 7,503 instrumented lines across the complete coverage artifact, including referenced assemblies at the artifact root).

New behavior covered:

- Shipment create, update, delete, issue, retrieval failure, activity logging, cache invalidation, persistence, and cancellation-token propagation.
- Shipment approval, rejection, pickup assignment, and delivery assignment, including missing entities and domain failures.
- Carrier status, deletion, availability, and additional-information workflows.
- Bundle subscribe/unsubscribe success, missing user, missing bundle, duplicate subscription, inactive subscription, persistence, and cache invalidation.
- Shipment by-id, statistics, tracking, and user-shipment query success/empty/not-found paths.
- Receiver, package-dimension, and shipment request validators, including enum validity, required values, address conflicts, and length boundaries.
- Validation, caching, idempotency, and transaction pipeline success/failure/bypass/cancellation contracts.

### Application gaps

- The prompt's per-handler minimum cannot yet be claimed across all 110 discovered handler files. Authentication, roles, most dashboards, ratings, trip workflows, and many carrier/user/location list and filter queries remain below the requested per-handler scenario count.
- Only a representative subset of the 81 validators has full boundary coverage.
- Pagination and filter composition remain incomplete across several query families.
- Logger verification is intentionally limited to behaviorally meaningful logging; technical log text is not treated as a business contract.

## Infrastructure coverage

The Infrastructure suite reports 21.30% line coverage and 4.63% branch coverage for `TransitNova.InfraStructure`. SQLite in-memory is used so keys, foreign keys, unique indexes, transactions, and relational persistence behavior are exercised.

New behavior covered:

- Outbox event conversion, payload/type restoration, event clearing, multi-event batches, and empty-event behavior.
- Processor ordering, 20-message batch cap, already-processed filtering, successful publishing, invalid type/JSON, publisher failures, partial-batch failure, and cancellation.
- Idempotency creation/existence/uniqueness/cancellation.
- Generic repository add, projection, no-tracking, not-found, predicate count, delete result, and explicit commit boundary.
- Audit-log tracking, persistence, nullable actor, multiple entries, and timestamp preservation.
- Country, city, zone, vehicle, refresh-token, shipment, bundle, idempotency, and activity-log model metadata: keys, conversions, unique/composite indexes, query filters, owned values, and delete behaviors.

### Infrastructure gaps

- Specialized shipment, trip, carrier, warehouse, identity, roles, refresh-token, bundle-subscription, admin, and analytics repository methods still need repository-specific suites.
- There is no dedicated outbox processor abstraction; the Quartz job performs querying, deserialization, publishing, and persistence itself.
- No production seed configuration was found, so seed-data tests would assert invented behavior and were not added.
- A full SQL Server integration suite is still needed for provider-specific clustered-index and row-version behavior.

## Risks revealed by tests

1. `ConvertDomainEventsToOutboxMessages` runs in `SavedChangesAsync`. It adds outbox rows only after the business save has completed, so the outbox message is not atomic with the business transaction and remains merely tracked until another save occurs.
2. `ProcessOutboxMessagesJob` marks all processed rows in one final save. If publication of a later message fails, an earlier message may already have been delivered while its processed marker is not persisted, causing duplicate publication on retry.
3. Unknown event types and invalid JSON propagate and block the batch; `OutboxMessage.Error` is never populated.
4. Idempotency is persisted before the handler executes. If the handler fails, the key remains recorded and a legitimate retry can be rejected as a duplicate.
5. `CachingBehaviour` invokes `next()` without forwarding the pipeline cancellation token; the test characterizes the current behavior.
6. The production Shipment/Bundle relationship still requires a test-only model correction because `PackageBundleId` and `Bundle.Id` use incompatible key types.
7. NuGet reports NU1903 for transitive `SQLitePCLRaw.lib.e_sqlite3` 2.1.11. This affects the test provider dependency chain and should be upgraded when a compatible patched chain is available.

## Minimal production-facing changes

- Added/retained `InternalsVisibleTo` access for the renamed `TransitNova.ApplicationLayer.Tests` and `TransitNova.InfraStructure.Tests` assemblies. This avoids widening production APIs solely for tests.
- No handler, entity, repository, outbox, or API behavior was changed.
- Added `Microsoft.EntityFrameworkCore.Sqlite` 10.0.6 to the Infrastructure test project only.

## Recommended next increment

1. Move outbox conversion to `SavingChangesAsync` (or an equivalent pre-commit transactional path) and add an atomicity regression test.
2. Make outbox processing per-message durable or explicitly transactional with an inbox/deduplication contract.
3. Record idempotency completion only after successful command execution, while representing in-progress requests separately.
4. Add repository-specific SQLite suites for shipment, trip, carrier, warehouse, identity, and role persistence.
5. Continue feature-by-feature handler coverage until every discovered handler meets the requested happy-path and failure-path minimums.
