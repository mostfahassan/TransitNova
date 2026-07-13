# CI Test Coverage

## Policy

TransitNova enforces code coverage from `build/coverage/coverage-thresholds.json`.

| Scope | Line gate | Branch gate | Current line | Current branch |
| --- | ---: | ---: | ---: | ---: |
| Domain | 80% | 70% | 88.04% | 70.92% |
| Application | 80% | 58% | 81.24% | 58.98% |
| Infrastructure | 80% | 50% | 80.14% | 51.33% |
| API | 80% | 53% | 85.62% | 55.10% |
| Payment | 80% | 65% | 87.40% | 66.27% |
| Overall | 80% | 58% | 82.31% | 58.45% |

Line coverage is an acceptance gate. Branch thresholds are regression floors and must rise with new branch coverage until they reach the 80% target recorded in the threshold file.

## How CI Generates Coverage

The `build-test-coverage` job in `.github/workflows/ci.yml`:

1. Restores and builds `TransitNova.slnx` in Release mode.
2. Runs Domain, Application, Infrastructure, API Integration, Payment, and Mapping tests once.
3. Collects one raw Cobertura file per test host with `XPlat Code Coverage`.
4. Passes all raw reports to ReportGenerator 5.5.10.
5. Filters the merged execution by architectural assembly into five reports.
6. Runs `build/coverage/Test-Coverage.ps1`.
7. Uploads HTML/Cobertura/text reports and TRX results even when a later gate fails.

Merging before assembly filtering is required. API integration and mapping tests execute Domain, Application, and Infrastructure code; generating each layer from only one test project under-reports real coverage.

## Approved Exclusions

The report excludes generated and configuration-only files:

- `Migrations`, model snapshots, designer files, and compiler/OpenAPI `obj` output.
- Main Infrastructure service registration, demo seed data, and design-time `AppDbContextFactory`.
- Payment startup `Dependencies`, database synchronization, and `Program`.

Do not add business services, repositories, handlers, validators, controllers, jobs, middleware, token, SignalR, outbox, reports, or caches to these exclusions.

## Run Locally

Install the pinned report tool:

```powershell
dotnet tool install --global dotnet-reportgenerator-globaltool --version 5.5.10
```

Build once:

```powershell
dotnet build TransitNova.slnx --configuration Release -m:1
```

Run each non-browser project with coverage into `artifacts/test-results/<name>` using the same command shape as CI:

```powershell
dotnet test Tests/TransitNova.Domain.Tests/TransitNova.Domain.Tests.csproj --configuration Release --no-build --results-directory artifacts/test-results/domain --collect:"XPlat Code Coverage" -- DataCollectionRunSettings.DataCollectors.DataCollector.Configuration.Format=cobertura
```

Repeat for Application, Infrastructure, API Integration, Payment, and Mapping. Then run the ReportGenerator group commands from `.github/workflows/ci.yml` and enforce the gates:

```powershell
./build/coverage/Test-Coverage.ps1
```

The gate script intentionally fails when a configured report is missing or malformed. A missing report is a collection failure, not zero coverage.

## Outputs

| Path | Contents |
| --- | --- |
| `artifacts/test-results/<suite>` | TRX and raw collector output |
| `artifacts/coverage/<layer>/Cobertura.xml` | Machine-readable gated report |
| `artifacts/coverage/<layer>/index.html` | Human-readable report |
| `artifacts/coverage/<layer>/Summary.txt` | Compact coverage summary |
| GitHub `coverage-reports` artifact | All layer reports |
| GitHub `test-results` artifact | All non-browser TRX results |
| GitHub `e2e-results` artifact | E2E TRX and Compose logs |

## Failure Conditions

CI fails when:

- Restore or Release build fails.
- Any test project fails.
- A raw collector or ReportGenerator command fails.
- Any configured report is missing or invalid.
- Any layer line coverage is below 80%.
- Weighted overall line coverage is below 80%.
- Any layer or overall branch coverage falls below its recorded regression floor.
- Compose services do not become healthy for full-stack E2E.
- Any browser or direct-API E2E scenario fails.

## Full-Stack E2E

The separate Ubuntu job builds `Tests/TransitNova.E2E.Tests`, installs the pinned Chromium runtime, starts the full Compose stack, waits for API, Payment, and UI health, runs all 55 browser and direct-API cases, captures logs, and removes containers and volumes.

The 55 discovered cases map to 39 of 48 required workflow groups (81.25% authored workflow coverage). Runtime success is established only by the E2E TRX; build/discovery alone is not reported as a passing E2E run.

Configure these GitHub Actions secrets:

- `TRANSITNOVA_CI_SQL_PASSWORD`
- `TRANSITNOVA_CI_JWT_KEY`
- `TRANSITNOVA_CI_PAYMENT_KEY`

Run locally after starting Docker Desktop:

```powershell
$env:SeedDemoData = "true"
$env:PaymentExecution__DelayMilliseconds = "0"
$env:PaymentExecution__ForcedSuccess = "true"
docker compose up --build -d
dotnet build Tests/TransitNova.E2E.Tests/TransitNova.E2E.Tests.csproj --configuration Release
pwsh Tests/TransitNova.E2E.Tests/bin/Release/net10.0/playwright.ps1 install chromium
$env:TRANSITNOVA_E2E_BASE_URL = "http://localhost:5169"
$env:TRANSITNOVA_E2E_API_BASE_URL = "http://localhost:5200"
$env:TRANSITNOVA_E2E_RUN_ID = "local-clean-stack"
dotnet test Tests/TransitNova.E2E.Tests/TransitNova.E2E.Tests.csproj --configuration Release --no-build
docker compose down -v
```

The E2E project is intentionally not part of the default solution test command because it requires the full external stack. CI executes it explicitly.
