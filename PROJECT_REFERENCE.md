# TransitNova Backend Project Reference

Last reviewed: 2026-06-30

This file is the working reference for future reviews of the TransitNova backend. It summarizes what the project does, how it is structured, the important runtime flows, current quality status, remaining review findings, and the learning roadmap.

## Current Verification Snapshot

Commands run from `D:\TransitNova`:

```bash
dotnet test TransitNova.slnx --no-restore
dotnet list TransitNova.slnx package --vulnerable --include-transitive
```

Results on 2026-06-30:

```text
dotnet test:
  Failed: 0
  Passed: 719
  Skipped: 0

package vulnerability scan:
  No vulnerable packages reported by configured NuGet sources.
```

Warnings still visible during test/build:

- Nullable warnings in zone update flow.
- Nullable warnings in auth response construction.
- Nullable warnings in token claim construction.

These are not failing the build, but they are worth cleaning before enabling warnings-as-errors.

## Project Purpose

TransitNova is a logistics backend that manages shipment lifecycle operations from user creation through shipment pricing, payment, shipment creation, approval, carrier assignment, pickup, warehouse transfer, delivery, rating, notification, and operational dashboards.

The backend is intentionally more advanced than basic CRUD:

- CQRS with MediatR.
- Pipeline behaviors for validation, caching, cache invalidation, transaction handling, and idempotency.
- Domain entities with behavior and invariants.
- Domain events with transactional outbox.
- Resource-based authorization.
- Refresh-token rotation.
- Separate simulated payment service.
- API endpoint catalog integration tests.
- Docker Compose runtime with SQL Server and Seq.
- OpenTelemetry and Serilog.

## Solution Map

```text
Src/TransitNova.Api
  HTTP API, controllers, endpoint metadata, authorization handlers, middleware, dependency registration.

Src/TransitNova.BusinessLayer
  Application layer. CQRS requests, handlers, validators, DTOs, pipeline behaviors, app services.

Src/TransitNova.Domain
  Entities, aggregate roots, domain events, domain exceptions, enums.

Src/TransitNova.InfraStructure
  EF Core DbContext, Identity, repositories, UnitOfWork, token generation, SignalR, outbox, Quartz.

TransitNovaPayment/TransitNovaPayment.API
  Separate payment gateway API.

TransitNovaPayment/TransitNovaPayment.Busieness
  Payment command/query handling, validation, payment process and payment method strategies.

TransitNovaPayment/TransitNovaPayment.Infrastructure
  Payment database, repositories, cache, health checks.

Tests
  Domain tests, application tests, infrastructure tests, payment tests, mapping tests, API integration tests.
```

## Runtime Services

Docker Compose services:

| Service | Purpose | Port |
| --- | --- | --- |
| `sqlserver` | SQL Server 2022 | `1433` |
| `transitnova_api` | Main backend API | `5200:80` |
| `transitnova-payment` | Payment gateway service | `5300:80` |
| `transitnova-ui` | MVC/UI project | `5169:80` |
| `transitnova-seq` | Centralized logs/traces UI | `8081:80`, `5341:5341` |

Important environment variables:

```text
SQL_PASSWORD
JWT_KEY
PaymentSettings__PublicKey
PaymentSettings__PrivateKey
ConnectionStrings__ApiDefaultConnection
ConnectionStrings__PaymentDefaultConnection
```

## Main API Startup

`Src/TransitNova.Api/Program.cs`

1. Registers all dependencies through `builder.Services.AddDependencies(builder.Configuration)`.
2. Registers Serilog through `builder.Host.AddSerilog()`.
3. Applies pending EF migrations unless environment is `Testing`.
4. Runs middleware through `app.UseDependencies()`.

`Src/TransitNova.Api/Dependencies.cs` registers:

- OpenAPI and Scalar.
- JWT configuration validation.
- Payment settings validation.
- Infrastructure and business services.
- JSON enum conversion.
- OpenTelemetry.
- Rate limiting.
- Exception handler and ProblemDetails.
- API versioning.
- Authorization policies.
- HttpClient.
- SignalR notification hub.
- Health checks.

## Application Pipeline

Registered in `Src/TransitNova.BusinessLayer/DependencyInjection.cs`:

1. `ValidationBehavior`
2. `CachingBehavior`
3. `CacheInvalidationBehavior`
4. `TransactionPipelineBehavior`
5. `IdempotentCommandPipelineBehavior`

Important behavior:

- Validation only runs for commands.
- Cache lookup only runs for requests implementing `ICachable`.
- Cache invalidation runs after the inner request finishes successfully.
- Transaction behavior wraps requests implementing `ITransactional`.
- Idempotent commands inherit from `IdempotentCommand<TResponse>`, which also marks them transactional.

Current strength:

- The order is sensible.
- Cache invalidation happens after successful transaction completion.
- Idempotency response persistence is part of the transactional flow.

Remaining improvement:

- Concurrent duplicate requests with the same idempotency key can both pass the initial read before one fails on the unique key. Add a concurrency test and either lock, handle unique-key conflict gracefully, or re-read the persisted response after conflict.

Relevant files:

- `Src/TransitNova.BusinessLayer/Common/Behaviors/IdempotentCommandPipelineBehavior.cs`
- `Src/TransitNova.BusinessLayer/Common/Behaviors/TransactionPipelineBehavior.cs`
- `Src/TransitNova.InfraStructure/EntityConfig/IdempotentTableConfiguration.cs`

## Domain Events and Outbox

Domain events are raised by aggregate roots and converted into outbox rows by:

`Src/TransitNova.InfraStructure/Common/Interceptors/ConvertDomainEventsToOutboxMessages.cs`

Outbox processing is handled by:

`Src/TransitNova.InfraStructure/BackgroundJobs/ProcessOutboxMessagesJob.cs`

Flow:

1. Entity raises domain event.
2. EF SaveChanges interceptor collects domain events.
3. Events are serialized into `OutboxMessage`.
4. Quartz job processes unprocessed messages.
5. Message is deserialized and published through MediatR.
6. Notification/event handlers perform side effects.
7. Processed messages are marked with `ProcessedOn`.

Strength:

- This is a senior-level pattern for avoiding side effects before persistence.

Improvement:

- Add a dead-letter or admin visibility path for outbox messages that exceed max retry count.
- Consider storing event type aliases instead of raw assembly-qualified names if versioned deployments become a concern.

## Authentication and Token Flow

JWT generation:

`Src/TransitNova.InfraStructure/Token/TokenGenerator.cs`

Current access token expiration:

```text
TokenGenerator.cs:72 - Expires = DateTime.UtcNow.AddHours(1)
```

Refresh token repository mapping:

`Src/TransitNova.InfraStructure/Repository/TokenRepository/RefreshTokenRepository.cs`

Refresh token business flow:

`Src/TransitNova.BusinessLayer/Services/TokenServices/TokenService.cs`

Current refresh endpoint:

`Src/TransitNova.Api/Controllers/Token/RefreshTokenController.cs`

Important current behavior:

- The refresh-token controller is protected by `[Authorize(Roles = Role.AllUsers)]`.
- It checks ownership using the authenticated principal plus the supplied refresh token.
- This means the endpoint requires a currently valid access token.

Review implication:

- This is safer than anonymous refresh in one way, but it weakens the normal purpose of refresh tokens because once the access token expires, the user cannot call the refresh endpoint.

Recommended redesign:

- Make refresh endpoint not depend on a valid access token.
- Validate the refresh token directly through the database.
- Use the token's `UserId` as the owner.
- Keep reuse detection and rotation.
- Optionally accept expired access token only for extracting principal, but never validate it as a normal authorized request.

## Payment Flow

Main API payment client:

`Src/TransitNova.BusinessLayer/Services/PaymentServices/PaymentService.cs`

Payment API route:

`TransitNovaPayment/TransitNovaPayment.API/Controllers/Payment/PaymentController.cs`

Current payment call:

```text
POST {PaymentSettings:BaseUrl}/api/v1/payments/pay
Header: X-PaymentKey
Body: CreatePaymentDto
```

Fix applied during this review:

- The main API previously called `/api/payments/pay`.
- The payment API route is versioned as `/api/v1/payments/pay`.
- The payment service and unit test were updated to use `/api/v1/payments/pay`.

Files changed:

- `Src/TransitNova.BusinessLayer/Services/PaymentServices/PaymentService.cs`
- `Tests/TransitNova.ApplicationLayer.UnitTest/Services/PaymentWorkflowTests.cs`

Recommended next test:

- Add an integration test that hosts the payment API or verifies route generation from the actual payment controller.

## API Surface Tests

API integration tests:

`Tests/TransitNova.Api.IntegrationTests/ApiEndpointIntegrationTests.cs`

Endpoint catalog:

`Tests/TransitNova.Api.IntegrationTests/Infrastructure/ControllerEndpointCatalog.cs`

Snapshot:

`Tests/TransitNova.Api.IntegrationTests/Infrastructure/ControllerEndpointCatalogSnapshot.cs`

Current endpoint count:

```text
ExpectedEndpointCount = 116
```

Strength:

- Public API changes are visible through checksum and count.
- Endpoints are executed through the actual HTTP pipeline.
- Protected endpoints are checked for anonymous 401 behavior.
- State-changing endpoints are checked for idempotency-key contracts.

This is better than the usual junior-level API testing approach.

## Remaining Review Findings

### P1 - Refresh token endpoint depends on a valid access token

Files:

- `Src/TransitNova.Api/Controllers/Token/RefreshTokenController.cs:12`
- `Src/TransitNova.InfraStructure/Token/TokenGenerator.cs:72`

Why it matters:

- The access token expires after one hour.
- The refresh endpoint requires `[Authorize(Roles = Role.AllUsers)]`.
- A user whose access token has expired cannot refresh using the refresh token.

Recommended fix:

- Redesign refresh to validate the refresh token itself, not the current access token.
- Keep reuse detection and rotation.
- Move ownership check into the refresh-token lookup result.

### P2 - Shipment pickup raises delivered event

Files:

- `Src/TransitNova.Domain/Entities/MainEntities/Shipment.cs:302`
- `Src/TransitNova.Domain/Entities/MainEntities/Shipment.cs:311`
- `Src/TransitNova.BusinessLayer/Common/Events/ShipmentEventsHandlers/ShipmentDeliveredDomainEventHandler.cs`

Why it matters:

- `PickedUp(Guid carrierId)` changes status to `PickedUp`.
- It raises `ShipmentDeliveredDomainEvent`.
- The handler creates a notification titled `Shipment Delivered`.
- This can notify users that a shipment was delivered when it was only picked up.

Recommended fix:

- Create `ShipmentPickedUpDomainEvent`.
- Add a handler with pickup-specific notification text.
- Keep `ShipmentDeliveredDomainEvent` only for final delivery.

### P2 - Idempotency duplicate concurrency behavior is not fully tested

Files:

- `Src/TransitNova.BusinessLayer/Common/Behaviors/IdempotentCommandPipelineBehavior.cs:24`
- `Src/TransitNova.BusinessLayer/Common/Behaviors/IdempotentCommandPipelineBehavior.cs:36`
- `Src/TransitNova.InfraStructure/EntityConfig/IdempotentTableConfiguration.cs:10`
- `Src/TransitNova.InfraStructure/EntityConfig/IdempotentTableConfiguration.cs:16`

Why it matters:

- Two identical requests can read "not found" at the same time.
- Both can execute the command.
- The unique key prevents duplicate idempotency records, but business side effects might already have happened before one request fails.

Recommended fix:

- Add a concurrent integration test.
- Catch unique-key conflict and re-read cached response.
- Consider serializable transaction or explicit lock only if the business operation requires strong exactly-once semantics.

### P3 - UnitOfWork transaction is not disposed or reset

File:

- `Src/TransitNova.InfraStructure/Repository/UnitOfWork.cs:11`
- `Src/TransitNova.InfraStructure/Repository/UnitOfWork.cs:30`
- `Src/TransitNova.InfraStructure/Repository/UnitOfWork.cs:36`

Why it matters:

- Current behavior works in scoped request flow.
- But the transaction object remains assigned after commit/rollback.
- This is a cleanup and robustness issue.

Recommended fix:

- Dispose transaction after commit/rollback.
- Set `_transaction = null`.
- Prevent nested `BeginTransactionAsync` from silently overwriting an existing transaction.

### P3 - Nullable warnings should be cleaned

Files:

- `Src/TransitNova.BusinessLayer/Features/Zones/Handlers/ApplyCommands/UpdateZoneHandler.cs`
- `Src/TransitNova.BusinessLayer/Features/Zones/Commands/CommandsValidators/UpdateZoneCommandValidator.cs`
- `Src/TransitNova.BusinessLayer/Features/UserAuthentication/Authentication/Handlers/ApplyingCommands/RegistrationHandler.cs`
- `Src/TransitNova.BusinessLayer/Features/UserAuthentication/Authentication/Handlers/ApplyingCommands/LoginHandler.cs`
- `Src/TransitNova.InfraStructure/Token/TokenGenerator.cs`

Why it matters:

- Tests are green, but warnings hide potential null-contract issues.
- Clean nullable contracts make the project feel more senior.

Recommended fix:

- Make DTO fields required where they are required by the domain.
- Avoid null-forgiving operators in token claims.
- Guard Identity-returned values before mapping to response DTOs.

## Loose Coupling Review

Good coupling decisions already present:

- Business layer depends on interfaces, not EF Core.
- Infrastructure implements repository and service abstractions.
- Domain has no dependency on infrastructure.
- Payment gateway is behind `IPaymentService`.
- Registration strategy pattern separates user-type-specific profile creation.
- Authorization policies use handlers instead of hard-coded controller checks in many places.

Places to loosen coupling over time:

1. System activity logs are written directly in many command handlers.
   - Better future direction: domain/application events create logs through handlers.

2. Cache invalidation keys are manually listed inside handlers.
   - Current approach is clear and explicit.
   - Future direction: centralize invalidation rules per command type or use event-based invalidation.

3. Payment response contract is represented by private nested classes in `PaymentService`.
   - Current approach is acceptable.
   - Future direction: move gateway contracts into a dedicated integration contract namespace or shared package if both services evolve together.

4. Authorization handlers sometimes use repository methods directly.
   - Current approach is normal.
   - Future direction: resource authorization services can group related checks if policies grow.

5. Outbox stores assembly-qualified event type names.
   - Current approach works.
   - Future direction: stable event type names or a registry reduce deployment/versioning coupling.

## Scoring Against Junior Baseline

Scores are from 1 to 10 and compare this project to a typical junior developer working alone on a backend portfolio project.

| Area | Typical junior | TransitNova | Notes |
| --- | ---: | ---: | --- |
| CQRS and MediatR structure | 3-4 | 8 | Clear commands/queries/handlers and pipelines. Some naming and consistency polish remains. |
| Clean Architecture layering | 4 | 7.5 | Good separation. Some application concerns still repeated in handlers. |
| Domain modeling | 3-4 | 7.5 | Entities have behavior and events. Some semantic event issues remain. |
| EF Core and repositories | 4-5 | 7 | Good use of projections, configs, repositories. Watch over-abstraction and transaction cleanup. |
| Transactions and consistency | 3 | 7 | Transaction pipeline and outbox are strong. Idempotency concurrency needs hardening. |
| Idempotency | 1-2 | 7 | Very rare in junior projects. Needs concurrency conflict handling. |
| Authentication and authorization | 4 | 7.5 | JWT, roles, permissions, resource handlers. Refresh endpoint design needs redesign. |
| Error handling | 3-4 | 7.5 | Domain exception mapping and ProblemDetails are good. Keep status codes aligned with API docs. |
| Testing | 3 | 8 | 719 tests, endpoint catalog, infrastructure tests, payment workflow tests. Add cross-service route tests. |
| Observability | 1-2 | 7.5 | Serilog, Seq, OpenTelemetry, health checks, correlation id. Good for junior level. |
| Docker and deployability | 3 | 7 | Compose has API, payment, UI, SQL, Seq. Production hardening remains. |
| Security awareness | 3 | 7 | JWT validation, permission policies, package scan. Refresh design and payment key model can mature. |
| Code consistency and polish | 4 | 6.5 | Strong system, but typos, nullable warnings, naming inconsistencies, and some duplication remain. |
| Overall backend level | 4 | 7.5-8 | Strong junior-plus / early mid-level backend portfolio, with a few important design issues to fix. |

Plain-language verdict:

You are not building like a normal junior CRUD developer anymore. The project shows serious learning and architectural ambition. The remaining issues are not "you do not know backend"; they are the kind of second-pass production details that appear once a project becomes large enough to have real integration and consistency concerns.

## Recommended Learning Roadmap

Focus topics to research next:

1. Refresh token security patterns
   - Rotation
   - Reuse detection
   - Expired access-token principal extraction
   - Device/session token families

2. Idempotency under concurrency
   - Unique-key conflict recovery
   - Exactly-once vs at-least-once semantics
   - Idempotency for payment/shipment creation
   - Race-condition tests

3. Transactional outbox patterns
   - Dead-letter queues
   - Stable event type names
   - Retry policies
   - Event versioning

4. EF Core production performance
   - Query splitting
   - Projection-first queries
   - Compiled queries
   - Concurrency tokens
   - Tracking vs no-tracking

5. API design maturity
   - REST resource naming
   - Versioning strategy
   - ProblemDetails consistency
   - Endpoint contract tests
   - Consumer-driven contract tests

6. Resilient HTTP integrations
   - Typed HttpClient
   - Timeouts
   - Retries with jitter
   - Circuit breakers
   - Idempotency across service boundaries

7. Security hardening
   - Secret management
   - Token storage strategy for browser UI
   - CSRF if cookies are used
   - CORS in production
   - Rate-limit partition design

8. Production observability
   - Trace correlation
   - Log sampling
   - Metrics and dashboards
   - Health checks with readiness/liveness split

9. Architecture refactoring
   - Event-driven side effects
   - Policy classes for cache invalidation
   - Reducing handler repetition
   - Module boundaries and vertical slices

10. Build quality
   - Warnings-as-errors
   - CI pipelines
   - Test coverage thresholds
   - Mutation testing for domain rules

## Suggested Next Backend Tasks Before UI Freeze

1. Redesign refresh endpoint behavior.
2. Add `ShipmentPickedUpDomainEvent` and handler.
3. Add idempotency concurrency test.
4. Clean nullable warnings.
5. Add payment API route integration/contract test.
6. Add transaction dispose/reset in `UnitOfWork`.
7. Add `.env.example`.
8. Add CI workflow that runs test and package vulnerability scan.

## Useful Commands

```bash
dotnet test TransitNova.slnx --no-restore
dotnet build TransitNova.slnx --no-restore
dotnet list TransitNova.slnx package --vulnerable --include-transitive
docker compose up --build
```

## Files Changed During This Review

Mechanical compile fix after namespace moves:

- Updated old `TransitNova.BusinessLayer.Interfaces.MarkerInterfaces` imports to `TransitNova.BusinessLayer.Common.Interfaces.MarkerInterfaces`.
- Updated `NotificationHub` import to `TransitNova.InfraStructure.SignalR.NotificationHubService`.

Payment route fix:

- `Src/TransitNova.BusinessLayer/Services/PaymentServices/PaymentService.cs`
- `Tests/TransitNova.ApplicationLayer.UnitTest/Services/PaymentWorkflowTests.cs`

Documentation added:

- `README.md`
- `PROJECT_REFERENCE.md`
