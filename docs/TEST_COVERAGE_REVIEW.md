# TransitNova Test Coverage Review

## Executive Summary

The verified Release baseline contains **862 passing non-browser tests**, with zero failures and zero skips. Production line coverage is **82.31% overall** and every measured production layer independently passes the **80% line gate**.

The full-stack E2E project now contains **55 discovered test cases**. They map to **39 of the 48 explicitly requested workflow groups**, producing **81.25% authored E2E workflow coverage**. The suite exercises the real UI, API, Payment API, SQL Server databases, outbox jobs, and SignalR hub through Docker Compose. The project builds successfully, but runtime execution is not claimed on this workstation because Docker Desktop is not running. GitHub Actions executes the full stack and publishes the TRX and Compose logs.

The original coverage measurement defect was corrected. CI now collects Cobertura from all six non-browser suites, merges the raw reports, and only then filters by production assembly. Cross-layer execution from API integration and mapping tests is therefore included rather than silently discarded.

## Coverage By Layer

| Layer | Previous line | Current line | Change | Current branch | Line gate |
| --- | ---: | ---: | ---: | ---: | ---: |
| Domain | 63.69% | 88.04% | +24.35 pp | 70.92% | 80% |
| Application | 40.79% | 81.24% | +40.45 pp | 58.98% | 80% |
| Infrastructure | 10.66% | 80.14% | +69.48 pp | 51.33% | 80% |
| API | 77.89% | 85.62% | +7.73 pp | 55.10% | 80% |
| Payment | 29.19% | 87.40% | +58.21 pp | 66.27% | 80% |
| **Overall** | Not previously enforced | **82.31%** | New weighted gate | **58.45%** | **80%** |

The percentages were remeasured on 2026-07-13 from the current Release binaries using XPlat Code Coverage and ReportGenerator 5.5.10.

## Test Inventory

| Test project | Cases | Verified result | Main coverage |
| --- | ---: | --- | --- |
| TransitNova.Domain.Tests | 121 | Passed | Aggregate invariants, state transitions, value objects, cache keys |
| TransitNova.ApplicationLayer.UnitTest | 519 | Passed | CQRS handlers, validators, services, pipelines, dashboards |
| TransitNova.InfraStructure.UnitTest | 143 | Passed | Relational repositories, PDF rendering, jobs, outbox, token, SignalR |
| TransitNova.Api.IntegrationTests | 25 | Passed | 142 endpoint contracts, authorization, errors, serialization, OpenAPI |
| TransitNova.Payment.Tests | 52 | Passed | Payment workflows, strategies, pipelines, caches, transactions |
| TransitNova.MappingTests | 2 | Passed | AutoMapper configuration and SQL-translatable projections |
| **Non-browser total** | **862** | **Passed** | Zero failed and zero skipped |
| TransitNova.E2E.Tests | 55 | Build/discovery verified; runtime pending | Browser and direct API workflows against the Compose stack |

## Test Changes

### New Tests

A total of **101 new non-browser test cases** were added:

- 60 Application cases for shipment creation, payment failures, bundle subscriptions, operational assignment/completion, filters, validators, vehicles, and dashboards.
- 18 Infrastructure cases for real QuestPDF output, relational invoice projections, report storage/cleanup, and report jobs.
- 21 Payment cases for validation/caching pipelines, cache implementations, payment methods, unit-of-work transactions, and deterministic payment execution configuration.
- 2 API integration cases for SignalR CORS preflight and anonymous hub authorization.

The new full-stack project contributes **55 E2E cases**. It started with 26 browser/navigation cases and was expanded with 29 API and business-workflow cases.

### Existing Tests Improved

Six existing test files were strengthened around warehouse handlers, DTO validation, shipment validation, EF entity configuration, and report generation, and integration-host configuration. API response snapshots were also reviewed and updated for intentional contract changes, including:

- the general admin subscribers endpoint;
- structured address DTOs;
- additional dashboard KPI fields;
- country metadata in location DTOs;
- the bundle report request endpoint.

Mapping tests now participate in Coverlet collection, and CI no longer executes a duplicate mapping run. SignalR runtime configuration was corrected as well: the notification bridge returns a browser-reachable API URL, Docker CORS uses the browser origin, the SignalR header is allowed, and routing executes before CORS. Integration tests lock down preflight and anonymous negotiate behavior.

## E2E Workflow Coverage

The denominator is the 48 workflow groups explicitly named in the testing objective. Theory rows for multiple roles count as one workflow capability, not as inflated coverage.

| Workflow group | Covered | Total | Evidence |
| --- | ---: | ---: | --- |
| Authentication | 4 | 4 | Login, refresh rotation/reuse, logout, anonymous access |
| Users and roles | 3 | 3 | Permissions, policies, role restrictions |
| Shipment lifecycle | 7 | 7 | Create, update, cancel, track, transitions, pricing, validation |
| Carrier flow | 2 | 4 | Profile update/cache invalidation and dashboard; create/assignment remain |
| Warehouse flow | 1 | 1 | Authenticated warehouse shipment/trip operational surfaces |
| Trip command flow | 0 | 3 | Read list/details covered, but create/update/assignment commands remain |
| Reports | 2 | 4 | Request/idempotency and invalid request; completed generation/download remain |
| Notifications | 2 | 2 | SignalR user isolation and two-stage outbox persistence/broadcast |
| Caching | 2 | 2 | Population and post-command invalidation |
| Idempotency | 2 | 2 | Safe retry and payload-hash conflict |
| Validation | 2 | 2 | Invalid DTO and invalid state/form behavior |
| Security | 3 | 3 | Authentication, authorization, permission boundaries |
| Error handling | 4 | 5 | Validation, business failure, not found, conflict; forced unexpected path remains |
| List query semantics | 4 | 4 | Pagination, filtering, sorting, search |
| Soft delete | 1 | 1 | Deleted shipment state persisted and queried |
| Concurrency | 0 | 1 | Full SQL race test remains |
| **Total** | **39** | **48** | **81.25% authored workflow coverage** |

Important full-stack scenarios now include:

- registration persistence followed by real login and duplicate-email conflict;
- refresh-token rotation and old-token reuse rejection;
- shipment create/payment invoice, idempotent retry, update, tracking, cancel, and soft delete;
- Admin and Operation Manager list/details response-contract validation;
- Carrier profile cache population, update, invalidation, ownership, and dashboard;
- deterministic sorting, filtering, search, and pagination;
- report request authorization/idempotency and invalid/not-found responses;
- five-role notification centers;
- a two-browser SignalR test that creates and approves a shipment, waits for outbox processing, verifies the persisted notification, verifies the live badge for the owner, and verifies isolation from another user.

## Determinism

The mocked Payment service previously used unconditional five-second delay plus Random.Shared success/failure, which made successful write E2E inherently flaky.

Payment execution now has validated configuration:

- default behavior remains a 5000 ms delay with randomized mock outcome;
- CI sets DelayMilliseconds to 0 and ForcedSuccess to true;
- forced success, forced failure, and cancellation are unit tested;
- E2E does not replace production repositories or payment services.

E2E registration data is derived from the CI run identity, and all asynchronous notification checks use bounded condition polling rather than arbitrary sleeps.

## Coverage Scope

Line coverage is covered executable lines divided by coverable executable lines. Branch coverage is covered branches divided by valid branches. Overall coverage is weighted from covered and valid counts across the five reports.

Approved exclusions are limited to:

- EF migrations, model snapshots, designer files, and obj output;
- Infrastructure service-registration code, demo seeding, and design-time DbContext factory;
- Payment startup composition, database synchronization startup, and Program.

Repositories, services, CQRS handlers, validators, background jobs, outbox, SignalR, token logic, PDF generation, middleware, controllers, and payment behavior remain in scope.

UI assemblies are validated through full-stack browser workflow coverage rather than in-process line instrumentation.

## Largest Remaining Uncovered Areas

| Layer | Highest remaining risk |
| --- | --- |
| Domain | ReportRequest, Notification, Trip, CacheKeys, Carrier, WarehouseManagerProfile |
| Application | TripManagementService, PaymentHistoryService, carrier-assignment event handlers, CompleteCarrierTripCommandHandler |
| Infrastructure | CarrierDashboardRepository, CarrierQueryRepository, role services, BundleSubscriptionQueryRepository, ZoneRepository |
| API | DatabaseSyncronyzation, GlobalExceptionHandler branches, ResultExtensions branches, carrier/role operation controllers |
| Payment | AppDbContext provider paths, bundle payment branches, health checks, remaining shipment-payment branches |

Branch coverage remains below 80%. The recorded branch thresholds are honest regression floors, while 80% remains the target. Passing an 80% line gate does not prove all concurrency interleavings or provider-specific SQL behavior.

## CI Enforcement

The GitHub Actions pipeline:

- restores and builds the Release solution;
- runs all six non-browser suites once;
- collects and merges Cobertura before layer filtering;
- generates HTML, Cobertura, and text reports;
- fails if any measured layer or overall line coverage is below 80%;
- enforces branch regression floors;
- uploads coverage and TRX artifacts;
- validates Docker Compose;
- builds the E2E project and installs Chromium;
- starts API, Payment, UI, SQL Server, and Seq with demo data;
- forces deterministic mocked payment only for E2E;
- runs all 55 full-stack cases;
- captures Compose logs even when startup or tests fail;
- tears down containers and volumes.

Required GitHub Actions secrets are TRANSITNOVA_CI_SQL_PASSWORD, TRANSITNOVA_CI_JWT_KEY, and TRANSITNOVA_CI_PAYMENT_KEY. No credential is stored in source control.

## Verification Status

Verified locally:

- Release solution build: zero warnings and zero errors.
- Non-browser tests: 862 passed, zero failed, zero skipped.
- Coverage gates: all passed.
- E2E build and discovery: 55 cases, zero build warnings/errors.
- Docker Compose syntax: valid.
- Payment strategy tests: 52 passed.

Not yet verified locally:

- Runtime execution of the 55 Compose-backed E2E cases, because Docker Desktop is not running.
- SQL Server provider-specific concurrency races.
- Successful background report generation and authenticated download.

## Remaining Work

1. Start Docker Desktop and execute the 55 full-stack E2E tests, then retain the TRX and Compose logs as runtime evidence.
2. Add successful report generation/download E2E after returning or discovering the generated report identifier.
3. Add SQL Server concurrency tests for bundle limits, idempotency, and competing shipment/trip transitions.
4. Add the missing carrier creation/assignment and trip command workflows when those public API operations are available.
5. Continue raising branch floors toward 80%, prioritizing Application and Infrastructure error/state branches.
