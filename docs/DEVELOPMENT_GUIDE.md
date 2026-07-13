# Development Guide

## Purpose

Provide a reproducible workflow for configuring, running, migrating, debugging, testing, and extending TransitNova.

## Prerequisites

- .NET SDK 10.x.
- Docker Desktop with Linux containers for the full stack.
- PowerShell 7 or Windows PowerShell.
- EF Core CLI tool aligned with runtime packages, preferably 10.0.9 for the audited state.
- An IDE with ASP.NET Core and container debugging support.

Install or update EF tooling:

```powershell
dotnet tool update --global dotnet-ef --version 10.0.9
```

## Repository Structure

```text
Src/TransitNova.Domain             Domain entities, events, enums, invariants
Src/TransitNova.BusinessLayer      CQRS, handlers, validators, DTOs, behaviors
Src/TransitNova.InfraStructure     EF Core, Identity, repositories, jobs, SignalR
Src/TransitNova.Api                Versioned REST API and HTTP concerns
UI/TransitNovaUI.BusinessLayer     Typed API clients and UI contracts
UI/TransitNova.UI                  MVC Areas, Razor views, JavaScript
TransitNovaPayment                 Independent mocked payment service
Tests                              Six default test projects plus the Compose-backed Playwright E2E project
build/coverage                     Coverage policy and enforcement script
.github/workflows/ci.yml           Build, test, mapping, Compose, coverage CI
docs                               Engineering and operating documentation
```

## Configuration

Create a local `.env` file at the repository root. It is ignored by Git. Use unique development-only secrets:

```dotenv
SQL_PASSWORD=<strong-local-sql-password>
JWT_KEY=<at-least-48-bytes-of-random-development-key-material>
PaymentSettings__PublicKey=<local-payment-public-key>
PaymentSettings__PrivateKey=<matching-local-payment-private-key>
ConnectionStrings__ApiDefaultConnection=Server=sqlserver,1433;Database=TransitNovaDb;User Id=sa;TrustServerCertificate=True
ConnectionStrings__PaymentDefaultConnection=Server=sqlserver,1433;Database=TransitNovaPaymentDb;User Id=sa;TrustServerCertificate=True
```

The database registration appends `SQL_PASSWORD` when the connection string has no password. Do not commit the resulting complete connection string.

To prevent automatic synthetic data in Development, set:

```powershell
$env:SeedDemoData = "false"
```

Enable the deterministic demo seed only for an intentional portfolio demonstration:

```powershell
$env:SeedDemoData = "true"
```

## Run with Docker

Validate configuration first:

```powershell
docker compose config --quiet
```

Build and start:

```powershell
docker compose up --build
```

Start in the background:

```powershell
docker compose up --build -d
```

Inspect service status and logs:

```powershell
docker compose ps
docker compose logs --tail 200 transitnova.api
docker compose logs --tail 200 transitnova-payment
docker compose logs --tail 200 transitnova-ui
```

Local endpoints:

| Service | URL |
| --- | --- |
| MVC UI | `http://localhost:5169` |
| Main API | `http://localhost:5200` |
| Payment API | `http://localhost:5300` |
| Main health | `http://localhost:5200/health` |
| Payment health | `http://localhost:5300/health` |
| SignalR hub | `http://localhost:5200/hubs/notifications` |
| Seq UI | `http://localhost:8081` |

## Run without Docker

Start SQL Server separately and provide host connection strings. Main infrastructure selects `HostContainerConnection` when `DOTNET_RUNNING_IN_CONTAINER` is not true.

Run each application in a separate terminal:

```powershell
dotnet run --project TransitNovaPayment/TransitNovaPayment.API/TransitNovaPayment.API.csproj
dotnet run --project Src/TransitNova.Api/TransitNova.Api.csproj
dotnet run --project UI/TransitNova.UI/TransitNova.UI.csproj
```

Set `PaymentSettings__BaseUrl` and `TransitNovaApi__BaseUrl` to the actual local HTTPS or HTTP launch-profile URLs when not using Compose.

## Database Migrations

List migrations without connecting:

```powershell
dotnet ef migrations list --no-connect `
  --project Src/TransitNova.InfraStructure/TransitNova.InfraStructure.csproj `
  --startup-project Src/TransitNova.Api/TransitNova.Api.csproj `
  --context AppDbContext
```

Create a Main migration:

```powershell
dotnet ef migrations add <MigrationName> `
  --project Src/TransitNova.InfraStructure/TransitNova.InfraStructure.csproj `
  --startup-project Src/TransitNova.Api/TransitNova.Api.csproj `
  --context AppDbContext
```

Apply Main migrations:

```powershell
dotnet ef database update `
  --project Src/TransitNova.InfraStructure/TransitNova.InfraStructure.csproj `
  --startup-project Src/TransitNova.Api/TransitNova.Api.csproj `
  --context AppDbContext
```

Create and apply Payment migrations with the corresponding Payment projects:

```powershell
dotnet ef migrations add <MigrationName> `
  --project TransitNovaPayment/TransitNovaPayment.Infrastructure/TransitNovaPayment.Infrastructure.csproj `
  --startup-project TransitNovaPayment/TransitNovaPayment.API/TransitNovaPayment.API.csproj `
  --context AppDbContext

dotnet ef database update `
  --project TransitNovaPayment/TransitNovaPayment.Infrastructure/TransitNovaPayment.Infrastructure.csproj `
  --startup-project TransitNovaPayment/TransitNovaPayment.API/TransitNovaPayment.API.csproj `
  --context AppDbContext
```

Review every generated migration and model snapshot. Do not hand-edit the snapshot. Include migration tests for constraints, indexes, and destructive changes.

## Build and Test

Restore and build the full solution:

```powershell
dotnet restore TransitNova.slnx
dotnet build TransitNova.slnx --configuration Release --no-restore -m:1
```

The single-worker setting prevents the local MSBuild and QuestPDF native-asset copy from exhausting limited workstation memory.

Run all test projects:

```powershell
dotnet test TransitNova.slnx --configuration Release --no-build
```

Run targeted suites:

```powershell
dotnet test Tests/TransitNova.Domain.Tests/TransitNova.Domain.Tests.csproj --configuration Release --no-build
dotnet test Tests/TransitNova.ApplicationLayer.UnitTest/TransitNova.ApplicationLayer.Tests.csproj --configuration Release --no-build
dotnet test Tests/TransitNova.InfraStructure.UnitTest/TransitNova.InfraStructure.Tests.csproj --configuration Release --no-build
dotnet test Tests/TransitNova.Api.IntegrationTests/TransitNova.Api.IntegrationTests.csproj --configuration Release --no-build
dotnet test Tests/TransitNova.MappingTests/TransitNova.MappingTests.csproj --configuration Release --no-build
dotnet test Tests/TransitNova.Payment.Tests/TransitNova.Payment.Tests.csproj --configuration Release --no-build
```

The audited local baseline is 862 passing non-browser tests. The Playwright project adds 55 Compose-backed browser/API E2E cases (81.25% authored workflow coverage) and runs separately.

## Coverage

GitHub Actions merges raw execution from all six non-browser suites, filters five production assembly groups, and generates `artifacts/coverage/<layer>/Cobertura.xml`. The enforcement script reads `build/coverage/coverage-thresholds.json`:

```powershell
./build/coverage/Test-Coverage.ps1
```

The script requires generated reports, enforces 80% line coverage for every layer and overall, checks branch regression floors, and writes a Markdown table to `GITHUB_STEP_SUMMARY`. See `docs/CI_TEST_COVERAGE.md` for local collection, approved exclusions, artifacts, secrets, and E2E execution.

## Debugging in Containers

1. Use a Debug container build or IDE Docker launch profile so PDB files match the running assemblies.
2. Start the complete dependency stack; breakpoints in Main API will not help if the MVC request fails before reaching the API or Payment API is unavailable.
3. Attach the debugger to the `dotnet` process inside the correct container: `TransitNova.Api.dll`, `TransitNova.UI.dll`, or `TransitNovaPayment.API.dll`.
4. Confirm source checksum and symbols in the IDE Modules window when a breakpoint is hollow.
5. Use container DNS names inside containers: `sqlserver`, `transitnova.api`, and `transitnova-payment`. Use `localhost` only from the host.
6. Follow the correlation ID across MVC, Main API, Payment API, and Seq.
7. Check `/health` before debugging a business handler.

For a Create Shipment issue, place breakpoints in this order:

1. `UserArea/ShipmentsController` POST Create action.
2. `UserShipmentsCommand` UI client.
3. Main `CreateShipmentCommandHandler`.
4. `ShipmentService` and `BundleBenefitService`.
5. `PaymentService` outbound call.
6. Payment `CreateShipmentPaymentHandler` and payment process.
7. Main invoice and shipment repositories before transaction commit.

## Troubleshooting

### `Failed to read Query response`

Inspect the API HTTP status and body, then find the `HttpHandler` warning containing status and a redacted body preview. Typical causes are a DTO mismatch, ProblemDetails returned where an envelope was expected, an empty failure body, or HTML from a proxy/error page. Do not suppress deserialization exceptions; align the response contract.

### Breakpoints are not hit

Verify the request URL points to the container instance being debugged, the running image was rebuilt, configuration is Debug, and matching PDBs are loaded. Recreate the application container after source changes.

### SQL connection fails from migrations

Host-run EF commands must use the mapped host endpoint. Container-run applications must use the Compose service name. Verify SQL health with `docker compose ps` and confirm `SQL_PASSWORD` matches the connection.

### Coverage script reports missing files

Run the coverage collection and ReportGenerator stages first. The enforcement script intentionally does not treat missing reports as zero because that would hide a broken CI collection step.

### OpenAPI snapshot fails

Review the received diff. Accept a snapshot only when the public change is intentional, version-compatible, and documented. Never update the snapshot solely to make CI green.

## Coding Standards

- Keep Domain independent from EF Core and HTTP types.
- Return DTOs, never EF entities or `IQueryable`, from public endpoints.
- Use UTC for persisted timestamps.
- Pass cancellation tokens through every async boundary.
- Use domain methods for state changes and raise events inside the aggregate.
- Mark transactional and idempotent commands explicitly.
- Scope every user-owned query from authenticated identity or a tested resource policy.
- Use `AsNoTracking` and projection for reads; load tracked aggregates only for commands.
- Add a maximum PageSize to paged queries.
- Add tests for success, expected failure, authorization, validation, and persistence behavior.
- Update OpenAPI snapshot, migration, coverage gate, and documentation when the contract changes.

## Adding a Feature

1. Define or extend a domain invariant and event when business behavior changes.
2. Add application DTOs, command or query, validator, handler, and repository contract.
3. Implement a projected read or tracked command repository method in Infrastructure.
4. Register dependencies through the feature registration extension.
5. Add role, permission, resource scoping, rate limiting, and idempotency at the API boundary.
6. Add or update the typed UI client and Area controller and view.
7. Add unit, repository, integration, mapping, serialization, and browser tests proportional to risk.
8. Generate and review a migration if persistence changes.
9. Run the full Release build, all tests, Compose validation, and coverage gate.

## Pull Request Checklist

- Release build passes with no new warnings.
- All six test projects pass.
- Authorization and ownership tests cover new routes.
- DTO and JSON changes pass serialization and mapping tests.
- OpenAPI snapshot change is intentional.
- Migration and snapshot are reviewed.
- No secret or real customer data is committed.
- Documentation and coverage baseline are updated where applicable.
