# Security Review

## Purpose

Assess TransitNova authentication, authorization, token handling, SignalR, payment-service trust, input validation, error disclosure, secrets, logging, Docker posture, demo data, and OWASP-aligned risks after the latest MVP hardening work.

## Scope

Main ASP.NET Core API, MVC UI authentication/session behavior, SignalR notification hub, Payment API, refresh tokens, role/permission claims, resource authorization, rate limiting, idempotency, ProblemDetails, Docker Compose, seeded demo data, and security regression coverage.

## Review Date

2026-07-13

## Executive Summary

TransitNova now has a stronger security baseline than the previous review. Several earlier release blockers were fixed: Warehouse Manager permission claims are now emitted in JWTs, unhandled 500 responses no longer publish raw exception messages as public ProblemDetails titles, `ValidationException` is consistently returned as 422, `ResultStatus.UnExpected` maps to HTTP 500, and successful 204 responses no longer include a response envelope body.

The remaining high-risk items are narrower but still important before claiming production readiness with real customer/payment data:

1. Refresh tokens are still stored and compared in plaintext. Reuse logging no longer prints the token value, but the database remains a bearer-secret store.
2. The Payment API still trusts a static `X-PaymentKey` header. It has no request signature, timestamp, nonce, rotation metadata, or replay protection.
3. The local Docker topology is intentionally development-oriented: Development environment, local Seq, and demo seed controls are not a production deployment model.

For a portfolio MVP with synthetic/demo data, the current posture is credible. For real production data, refresh-token hashing and payment request signing are still mandatory.

## Current Implementation

### Authentication

- Main API uses JWT bearer authentication with strict issuer, audience, lifetime, signing-key validation, and zero clock skew.
- Access tokens are signed with HMAC SHA-384 and expire after one hour.
- Refresh tokens are generated from 32 cryptographically random bytes and Base64 encoded.
- MVC UI uses HTTP-only auth cookies, server session, and same-site cookie behavior suitable for same-origin dashboard flows.
- SignalR notification hub is authenticated and targets users through the authenticated NameIdentifier claim.

### Authorization

- Controllers combine role authorization with named permission policies.
- JWT generation now includes permissions for User, Carrier, Operation Manager, Warehouse Manager, and Admin roles.
- Shipment, carrier, warehouse manager, and token ownership checks use resource-level authorization or scoped repository predicates.
- Operation Manager shipment/trip queries are scoped by the authenticated manager/profile instead of trusting client-supplied IDs.
- Notification APIs derive user scope from `User.GetUserId()` only.

### Abuse Controls

- Default fixed-window limiter is configurable and currently defaults to 100 requests per minute.
- Command limiter is configurable and currently defaults to 20 requests per minute.
- CI can raise limiter values for deterministic E2E execution without changing source defaults.
- Idempotent commands persist request hash and response data to prevent accidental duplicate command processing.

### Error Handling

- Unhandled server errors return a generic public title.
- Detailed exception information is logged with trace ID and is not returned outside Development.
- FluentValidation exceptions and MediatR validation failures now consistently map to 422.
- 204 No Content responses are empty, not envelope-wrapped.

### Demo Data

- Demo seeding is disabled by default in Docker Compose through `SeedDemoData=false`.
- When enabled, the seeder requires real `Countries`, `Governments`, and `Cities` lookup data to already exist.
- All seeded accounts intentionally share one documented demo password: `TransitNova@12345`.
- Demo credentials must never be enabled in a production environment.

## Remediated Since Previous Review

| Previous finding | Current status | Notes |
| --- | --- | --- |
| Missing Warehouse Manager permission claims | Fixed | `TokenGenerator` now adds `WarehouseManagerPermissions.All`. |
| Public 500 title used `exception.Message` | Fixed | 500 title is now generic and exception details stay in logs. |
| Validation status mismatch between thrown and pipeline validation | Fixed | `ValidationException` maps to 422. |
| `ResultStatus.UnExpected` fell through to 400 | Fixed | Unexpected result now maps to HTTP 500. |
| 204 response envelope body | Fixed | `NoContentResult` is used for success/no-content. |
| Demo seed default in Compose | Improved | Compose default is `SeedDemoData=false`; explicit opt-in is required. |
| Demo seeding inconsistent with external location data | Fixed by contract | Seeder now requires preloaded real location lookups instead of faking geography. |
| Refresh-token reuse log printed full token | Improved | Critical log now includes user ID only, not token material. |

## Security Analysis

### Refresh Token Storage

`RefreshToken.Token` and `RefreshToken.ReplacedByToken` are still persisted as plaintext bearer secrets. Repository lookup compares the supplied token directly against the stored token. If the database is disclosed, active refresh tokens can be replayed until expiry or revocation.

The reuse log no longer prints the token value, which reduces log leakage. However, the storage model remains the primary risk. Store only a keyed hash or digest of refresh tokens, compare digests, and store replacement relationships through internal IDs or digests rather than raw token strings.

### Payment API Trust Boundary

The Payment API receives `X-PaymentKey` and compares it with configuration. This is acceptable only for the mocked/local payment service. It does not prevent replay, does not bind the request body to the credential, and does not provide request freshness.

For production-like claims, sign each request with HMAC over method, path, body hash, timestamp, nonce, and idempotency key. The Payment API should reject stale timestamps and duplicate nonces.

### Error and Logging Disclosure

The Main API now returns generic titles for 500 responses and logs exceptions with trace ID. Remaining logging concerns:

- UI HTTP handler invalid-response previews can include response body fragments.
- SignalR `access_token` query strings can appear in proxy logs unless query logging is redacted.
- Payment and API logs should classify/redact headers, tokens, cookies, and request/response previews before central shipping.

### Secrets and Configuration

Tracked configuration does not embed production secrets. Runtime values are injected by environment variables. Startup validation exists for critical options. For production, environment variables alone are not enough: use a managed secret store, per-environment keys, key rotation, and least-privilege database credentials.

### Docker and Observability

The Compose stack is a development environment. It runs apps with `ASPNETCORE_ENVIRONMENT=Development` and exposes Seq locally. This is acceptable for local MVP validation, but not for deployment. Production containers should run non-root, use pinned base images or digest policy, and ship logs to authenticated observability infrastructure.

## OWASP Coverage

| OWASP area | Current control | Residual concern |
| --- | --- | --- |
| Broken access control | Roles, permissions, ownership handlers, scoped queries | Continue endpoint-level regression tests for every role/area. |
| Cryptographic failures | Strong JWT signing, random refresh-token generation | Refresh tokens are still plaintext at rest. |
| Injection | EF Core parameterization, DTO validation | Leading-wildcard search is mostly performance risk, not SQL injection. |
| Insecure design | Domain invariants, idempotency, outbox, scoped operations | Payment replay protection is not implemented. |
| Security misconfiguration | Options validation, HSTS, constrained CORS | Dev Compose and local Seq are not production-safe. |
| Vulnerable components | Central package versions | CI should add package, secret, and container scanning. |
| Authentication failures | Identity, JWT validation, refresh rotation, rate limits | Plaintext refresh-token storage remains. |
| Data integrity failures | Request hashes, row versions, outbox | Payment request authenticity and nonce replay are missing. |
| Logging/monitoring failures | Serilog, Seq, trace IDs | Redaction and alerting policy needs formalization. |
| SSRF | Payment base URL is configuration-bound | Production should restrict allowed payment hosts/workload identity. |

## Strengths

- JWT validation is strict and uses zero clock skew.
- Permission claims now cover all platform roles.
- SignalR hub requires authentication and sends user-specific payloads.
- Result and exception handling are more consistent and less leaky.
- Main API CORS is constrained to configured MVC origins.
- Rate limiting and idempotency are applied to state-changing flows.
- Demo seed is explicit and documented.
- Security and contract regression tests exist.

## Weaknesses

- Refresh tokens remain bearer secrets at rest.
- Payment service authentication is replayable and static.
- Development Compose must not be promoted to production.
- No dependency-vulnerability, secret, or container scan is currently enforced as a hard CI gate.
- Log redaction policy is not centralized across API, UI, Payment, SignalR, and proxies.

## Risk Register

| Priority | Finding | Security consequence | Status |
| --- | --- | --- | --- |
| High | Plaintext refresh tokens and replacement token values | Account takeover after database disclosure | Open |
| High | Static replayable payment key | Forged or replayed payment operations after key leakage | Open |
| Medium | Dev Compose/Seq topology | Sensitive diagnostics exposure if deployed unchanged | Open, local-only documented |
| Medium | Response preview/query-string logging | Token or user-data disclosure in logs | Open |
| Medium | Demo seed password | Predictable credentials if enabled outside demo | Mitigated by explicit opt-in/documentation |
| Low | No supply-chain scans in CI | Known vulnerable dependency may merge unnoticed | Open |

## Recommendations

### High Priority

1. Hash refresh tokens at rest and store replacement relationships without raw token values.
2. Add signed, timestamped, nonce-protected payment requests plus provider-side idempotency checks.
3. Add security regression tests for refresh-token hashing, payment signature rejection, and replay prevention.

### Medium Priority

1. Add package vulnerability scanning, secret scanning, and image scanning to CI.
2. Add centralized logging redaction for tokens, cookies, authorization headers, query strings, and response previews.
3. Create a production deployment profile separate from Docker Compose, with non-root containers and authenticated observability.
4. Enforce `SeedDemoData=false` outside Development through startup guardrails.

### Low Priority

1. Add key IDs and rotation procedures for JWT/payment credentials.
2. Review cookie lifetime and SameSite policy against the final deployment topology.
3. Add a lightweight threat model for shipment ownership, payment replay, notification targeting, and admin operations.

## Verification Snapshot

Latest non-browser verification completed after the recent changes:

| Check | Result |
| --- | ---: |
| Release solution build | Passed, 0 warnings, 0 errors |
| Domain tests | 121 passed |
| Application tests | 519 passed |
| Infrastructure tests | 144 passed |
| Payment tests | 52 passed |
| Mapping tests | 2 passed |
| API integration tests | 25 passed |
| EF pending model changes | None |

## Production Readiness

Security readiness is suitable for a portfolio MVP with synthetic or demo data. It is not ready for real customer or payment data until refresh-token hashing and signed payment requests are implemented.

## Overall Score

**7.0/10**

## Final Verdict

TransitNova has a deliberate and improving security architecture. The previous broad blockers around permission claims and error disclosure are closed. The remaining production blockers are focused: refresh-token storage and payment trust. Fix those before making a real production-data claim.
