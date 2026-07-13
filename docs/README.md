# TransitNova Engineering Documentation

This directory is the engineering review and operating documentation for TransitNova. It was produced from the repository state audited on 2026-07-13 and is intended for maintainers, reviewers, and portfolio evaluators.

## Verified Baseline

| Evidence | Result |
| --- | --- |
| Target framework | .NET 10 (`net10.0`) |
| Solution projects | 15, including six test projects and the Docker Compose project |
| Main API contract | 121 paths and 142 operations |
| Automated tests | 862 non-browser tests passed; 55 full-stack E2E cases authored (81.25% workflow coverage) |
| Release build | Passed with one MSBuild worker |
| Docker Compose syntax | `docker compose config --quiet` passed |
| Main database migration | `20260712073348_InitialMigration` |
| Payment database migration | `20260712073648_InitialMigration` |

The build initially exhausted local memory when MSBuild used parallel workers. The same build passed with `-m:1`. The GitHub Actions workflow now uses that deterministic setting.

## Document Map

| Document | Primary question answered |
| --- | --- |
| [ARCHITECTURE.md](ARCHITECTURE.md) | How is the system structured and how do requests and events move through it? |
| [PROJECT_REVIEW.md](PROJECT_REVIEW.md) | How strong is the project overall, and what is its honest readiness score? |
| [ARCHITECTURE_REVIEW.md](ARCHITECTURE_REVIEW.md) | Which architecture choices work, and where are the structural risks? |
| [CODE_QUALITY_REVIEW.md](CODE_QUALITY_REVIEW.md) | What concrete maintainability and correctness issues exist in the code? |
| [PERFORMANCE_REVIEW.md](PERFORMANCE_REVIEW.md) | Which query, concurrency, caching, and background-processing paths can become bottlenecks? |
| [SECURITY_REVIEW.md](SECURITY_REVIEW.md) | How are authentication, authorization, secrets, tokens, and abuse controls implemented? |
| [TEST_COVERAGE_REVIEW.md](TEST_COVERAGE_REVIEW.md) | What is tested, what is the measured coverage, and what remains untested? |
| [CI_TEST_COVERAGE.md](CI_TEST_COVERAGE.md) | How does CI collect, merge, gate, and publish coverage and E2E evidence? |
| [DATABASE_DESIGN.md](DATABASE_DESIGN.md) | How are entities, relationships, indexes, concurrency, and migrations designed? |
| [API_DESIGN.md](API_DESIGN.md) | How consistent and maintainable is the HTTP contract? |
| [DEPLOYMENT.md](DEPLOYMENT.md) | How are Docker, CI, migrations, health checks, and observability operated? |
| [DEVELOPMENT_GUIDE.md](DEVELOPMENT_GUIDE.md) | How does a developer build, run, debug, migrate, and test the solution? |

## Review Classification

The repository is a strong portfolio MVP. It demonstrates substantially more engineering depth than a CRUD sample: role-scoped workflows, CQRS, idempotency, transactions, an outbox, background reports, a separate payment service, SignalR, OpenAPI contract tests, and six automated test suites.

It is not approved for unattended production deployment in its current state. The review identified high-priority work in EF Core dashboard concurrency, refresh-token handling, Warehouse Manager permission claims, database indexing for notifications and the outbox, payment-service migration startup, and browser-level workflow testing.

## Evidence Policy

Statements in these documents are based on repository files, generated OpenAPI, Cobertura reports, or commands executed against the workspace. Recommendations are labeled High, Medium, or Low Priority. A score describes the audited state, not the intended design.

## Ownership

Update this documentation in the same pull request when changing public routes, authentication, data ownership rules, migrations, Docker topology, coverage gates, or cross-cutting pipeline behaviors.
