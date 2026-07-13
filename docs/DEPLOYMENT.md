# Deployment Review and Runbook

## Purpose

Describe the implemented Docker and CI topology, required configuration, migration procedure, health and observability behavior, and the changes required for a production deployment.

## Scope

`docker-compose.yml`, the three Dockerfiles, Main and Payment startup, GitHub Actions at `.github/workflows/ci.yml`, SQL Server, Seq, OpenTelemetry, health checks, migrations, and deployment safety.

## Executive Summary

The repository has a complete local container topology and an active GitHub Actions workflow. Docker Compose syntax was validated successfully with `docker compose config --quiet`. Multi-stage Dockerfiles build Main API, MVC UI, and Payment API. SQL Server has a readiness health check, and application services expose health endpoints and emit Serilog and OpenTelemetry data.

The Compose file is a Development environment, not a production manifest. It enables Development mode, exposes SQL Server and an unauthenticated Seq UI, uses mutable image tags, runs containers without an explicit non-root user, and does not define health checks for application containers. Main and Payment migration behavior is inconsistent.

## Current Implementation

### Container Topology

| Service | Image or Dockerfile | Host port | Dependency |
| --- | --- | ---: | --- |
| `sqlserver` | `mcr.microsoft.com/mssql/server:2022-latest` | 1433 | None |
| `transitnova-payment` | `TransitNovaPayment/Dockerfile` | 5300 | Healthy SQL Server |
| `transitnova.api` | `Src/Dockerfile` | 5200 | Healthy SQL Server and started Payment API |
| `transitnova-ui` | `UI/Dockerfile` | 5169 | Main API |
| `transitnova-seq` | `datalust/seq:latest` | 8081 and 5341 | None |

Named volumes persist SQL Server and Seq data. All services share the `transitnova` bridge network. Container-to-container URLs correctly use service DNS names rather than localhost.

### Required Environment Variables

| Variable | Consumer | Purpose |
| --- | --- | --- |
| `SQL_PASSWORD` | SQL Server and connection-string completion | Local SA password |
| `JWT_KEY` | Main API | HS384 signing key; at least 48 bytes |
| `PaymentSettings__PublicKey` | Main API | Key sent to mocked Payment API |
| `PaymentSettings__PrivateKey` | Payment API | Expected mocked payment key |
| `ConnectionStrings__ApiDefaultConnection` | Main API | Container SQL connection |
| `ConnectionStrings__PaymentDefaultConnection` | Payment API | Container SQL connection |

`.env` is intentionally not tracked. Production secrets must come from a managed secret provider and must not be baked into images or repository files.

### Health and Observability

Main API maps `/health` and registers database, payment-gateway configuration, and observability configuration checks. Payment API also maps `/health` and receives infrastructure health registrations. Serilog request logging is enabled in both APIs; UI and both APIs register OTLP tracing. Compose sends traces to Seq's OTLP ingestion endpoint.

Health endpoints currently provide service-level status, but Compose does not use application health checks for readiness ordering. Add HTTP health checks and make UI depend on a healthy Main API and Main API depend on a healthy Payment API.

### CI Workflow

The CI definition is [`.github/workflows/ci.yml`](../.github/workflows/ci.yml). It runs on pushes to `main`, `develop`, and `codex/**`, and on pull requests.

Current stages:

1. Checkout with persisted credentials disabled.
2. Install .NET 10.
3. Restore `TransitNova.slnx`.
4. Release build with `-m:1` for predictable memory use.
5. Validate `docker-compose.yml` with non-production CI placeholder settings.
6. Run AutoMapper configuration and SQL-translation tests.
7. Install ReportGenerator.
8. Run Domain, Application, Infrastructure, API, and Payment test projects with Cobertura collection.
9. Generate filtered reports per layer.
10. Enforce line and branch regression gates through `build/coverage/Test-Coverage.ps1`.
11. Upload coverage and test artifacts even when a later step fails.

## Migration Procedure

### Main Database

```powershell
dotnet ef database update `
  --project Src/TransitNova.InfraStructure/TransitNova.InfraStructure.csproj `
  --startup-project Src/TransitNova.Api/TransitNova.Api.csproj `
  --context AppDbContext
```

### Payment Database

```powershell
dotnet ef database update `
  --project TransitNovaPayment/TransitNovaPayment.Infrastructure/TransitNovaPayment.Infrastructure.csproj `
  --startup-project TransitNovaPayment/TransitNovaPayment.API/TransitNovaPayment.API.csproj `
  --context AppDbContext
```

For host-executed migrations, use connection strings that target the mapped SQL port, normally `localhost,1433`. Inside containers, target `sqlserver,1433`.

Main API currently applies migrations automatically at startup and retries ten times with five-second delays. It also seeds roles and can seed demo data. Payment has an equivalent migration helper, but its `Program.cs` does not invoke it. A production pipeline should apply both migrations once before application rollout and run applications with database identities that cannot alter schema.

## Engineering Analysis

### Dockerfile Strengths

- Multi-stage builds separate SDK and ASP.NET runtime images.
- Project files and central package files are copied before restore for layer caching.
- Publish uses Release configuration and disables app host where configured.
- Runtime images contain published output rather than source trees.

### Dockerfile and Compose Gaps

- Base images use mutable `10.0` or `latest` tags rather than pinned patch versions or digests.
- No `USER` instruction drops root privileges.
- No read-only root filesystem, capability restrictions, or resource limits are declared.
- Application containers lack Docker health checks.
- All apps run in Development.
- Seq first-run authentication is disabled and its UI is published to the host.
- SQL Server is published on all host interfaces unless the host firewall restricts it.
- Seq environment list entries using colon syntax for exporter settings should be removed or normalized; Seq is the receiver, while application containers are the exporters.

### CI Reproducibility

Central package management improves reproducibility, but ReportGenerator is installed without a pinned version and GitHub Actions references moving major tags rather than commit SHAs. Pin tooling versions for a controlled release pipeline. Add dependency and container scanning before production.

## Strengths

- A complete one-command local topology exists.
- Container service URLs and database connections are separated from host URLs.
- SQL Server startup is health-gated.
- Health, structured logging, correlation IDs, and traces are integrated.
- CI builds, tests, validates mapping and Compose, gates coverage, and retains artifacts.
- Configuration options fail startup when critical keys are missing.

## Weaknesses

- Compose is Development-only and insecure for public deployment.
- Main and Payment migrations do not have one owner.
- Application container readiness is not modeled.
- Images are mutable and containers remain root.
- No deployment environment, registry publishing, rollback automation, or infrastructure-as-code is present.
- CI does not run browser tests, SQL Server migration tests, vulnerability scans, or image builds.

## Risks

- A fresh Payment deployment can start without tables.
- Multiple Main API replicas can race while applying migrations.
- An accidentally exposed Seq instance can reveal logs and traces.
- Mutable images can change between builds.
- A container compromise has broader impact when running as root.
- CI can pass while Docker images fail to build because image build is not executed.

## Trade-offs

A single Development Compose file is appropriate for local onboarding and a CV demo. It should remain clearly labeled and should not accumulate production conditionals. Create a separate production deployment definition when a real hosting target is selected.

## Metrics

| Metric | Value |
| --- | ---: |
| Docker services | 5 |
| Application Dockerfiles | 3 |
| Persistent volumes | 2 |
| Health endpoints | 2 API endpoints |
| CI coverage test projects | 5 |
| CI mapping test projects | 1 |

## Production Readiness

Deployment readiness is good for local development and controlled demonstrations. It is not production-ready without hardened images, managed secrets, migration orchestration, readiness probes, and a real hosting pipeline.

## Recommendations

### High Priority

1. Apply both database migrations in one deployment stage and remove application schema-change privileges.
2. Add Payment migration execution immediately for local correctness until deployment migration ownership is implemented.
3. Create production deployment configuration with managed secrets, TLS, authenticated observability, and private database networking.
4. Add application health checks and readiness dependencies.

### Medium Priority

1. Pin runtime, SQL Server, Seq, ReportGenerator, and action versions.
2. Run containers as non-root and apply resource and filesystem restrictions.
3. Add CI Docker image builds, SQL Server migration smoke, Playwright smoke, dependency audit, secret scan, and image scan.
4. Publish images with immutable commit and semantic-version tags.

### Low Priority

1. Add automated release notes and artifact retention policy.
2. Add a documented rollback and database restore drill.

## Future Improvements

Select a hosting target, codify infrastructure, add blue-green or rolling deployment safety, and define SLOs for API availability, payment latency, outbox age, and report completion.

## Overall Score

**6.5/10**

## Final Verdict

The local and CI foundations are strong for a portfolio MVP. A separate production deployment design is still required; the current Compose file must never be presented as production configuration.
