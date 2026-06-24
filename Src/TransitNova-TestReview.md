# TransitNova Test Review

Date: 2026-06-23

## Final Test Result

- Total test projects scanned: 9
- Test projects in solution run: 5
- Legacy empty projects checked separately: 2
- Total tests discovered: 671
- Total tests executed: 671
- Passed: 671
- Failed: 0
- Skipped: 0

Final command:

```powershell
dotnet test TransitNova.slnx --configuration Release --no-restore --logger "trx;LogFileName=TransitNovaTests.trx"
```

Final result: green.

## Coverage Summary Per Layer

- Domain: 115 tests passed. Covers domain entities, invariants, value behavior, and core business state transitions.
- Application: 431 tests passed. Covers validation behavior, cache behavior, transaction behavior, idempotency behavior, command/query handlers, not-found paths, duplicate request behavior, and DTO validators.
- Infrastructure: 116 tests passed. Covers EF Core configuration, repositories, outbox interceptor behavior, background job success/failure paths, retry handling, and persistence behavior.
- Mapping: 2 tests passed. Covers AutoMapper configuration validity.
- API integration support: 7 tests passed. Verifies endpoint catalog stability, authorization expectations, health endpoint, idempotency header contracts, and HTTP pipeline execution.

No line/branch coverage collector was run as part of this stabilization pass. The summary above is based on the automated test suite by layer.

## Fixed Defects

- Fixed idempotency cached-response deserialization by making `BaseResult` and `Result<T>` JSON-deserializable.
- Fixed `CreateCityHandler` null navigation access. The handler no longer reads `city.Government.Name` immediately after creating a new `City` entity without loading the navigation.
- Fixed vehicle create mapping so AutoMapper does not overwrite the domain factory-trimmed `PlateNumber`.
- Fixed API integration test host setup by referencing the API project and exposing the minimal-hosting `Program` type.
- Fixed startup migration behavior for tests by skipping SQL Server migrations in the `Testing` environment. Docker/development startup migrations remain enabled.
- Fixed API integration database initialization race with a deterministic `SemaphoreSlim` guard.
- Fixed malformed legacy `DomainLayerTesting.csproj` so it is loadable.

## Added Or Strengthened Tests

- Added duplicate idempotency coverage: stored response is returned and the handler is not executed again.
- Added idempotency corrupted-cache failure coverage.
- Strengthened idempotency new-request coverage to verify the persisted response is real serialized JSON.
- Strengthened idempotency handler-failure coverage to verify failed handler execution is not recorded as a completed idempotent request.
- Added background job retry-limit coverage.
- Updated background job failure-path tests for unknown event type, invalid JSON, publisher failure, and partial batch failure.
- Updated outbox interceptor tests to assert the current same-save persistence behavior.

## Failing Areas Before Fixes And Resolution

- Idempotency pipeline tests failed because tests expected placeholder strings and conflict behavior. Resolution: aligned tests with the actual idempotent retry contract and fixed result deserialization.
- `CreateCityHandler` failed with `NullReferenceException`. Resolution: stopped reading unloaded `Government` navigation during create response construction.
- Vehicle create/mapping tests failed because whitespace was preserved after AutoMapper mapping. Resolution: prevented AutoMapper from overwriting the domain factory-normalized plate number.
- Bundle update test failed due to using a random bundle id instead of the arranged entity id. Resolution: corrected the test setup.
- Outbox interceptor tests expected messages to remain `Added` after `SaveChanges`. Resolution: updated tests to match the interceptor's `SavingChangesAsync` behavior, where messages persist in the same save.
- Background job tests expected exceptions to propagate. Resolution: updated tests to match production job behavior: failures are captured on the message with retry metadata and processing continues.
- Vehicle EF configuration test expected `SetNull`, while production configuration uses `Restrict`. Resolution: aligned test expectation with the configured relationship.
- API integration tests failed due to missing API project reference and `testhost.deps.json`. Resolution: referenced the API project and removed the accidental testhost `Program` import.
- API integration tests failed under SQLite because startup migrations attempted SQL Server-specific migration SQL. Resolution: skipped startup migrations only in `Testing`.
- API integration initialization was flaky under parallel execution. Resolution: serialized one-time SQLite schema initialization.

## Remaining Risks

- `SQLitePCLRaw.lib.e_sqlite3` reports NU1903 high-severity vulnerability in test dependencies.
- SQLite integration tests do not fully emulate SQL Server-specific migration SQL or relational behavior.
- Two legacy projects, `DomainLayerTesting` and `BuisnessLayerTesting`, are loadable but contain no tests and are not part of the solution run.
- API pipeline tests use generic generated requests; they verify routing and status stability, not every endpoint's full business-success workflow.
- No formal line or branch coverage percentage was generated.
