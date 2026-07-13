# Performance Review

## Purpose

Evaluate database access, EF Core behavior, pagination, projection, caching, memory use, background processing, and likely production bottlenecks.

## Scope

Main and Payment data paths, dashboard aggregation, shipment and trip filters, notifications, outbox processing, reports, HTTP calls, and process-local caching.

## Executive Summary

TransitNova uses several sound performance practices: read queries usually use `AsNoTracking`, list endpoints paginate, projections execute in SQL, bulk updates use `ExecuteUpdateAsync`, and high-use shipment and trip fields have indexes. These choices keep normal MVP traffic efficient.

The highest-priority issue is unsafe, not merely slow: dashboard services execute multiple operations concurrently on one scoped `AppDbContext`. Other growth bottlenecks are missing notification and outbox indexes, leading-wildcard shipment search, process-local cache, sequential outbox publication, and memory-intensive PDF generation without explicit workload limits.

## Current Implementation

- `ShipmentQueryRepository.FilterAsync` performs filtered `COUNT(*)`, then a projected ordered page.
- Shipment, Trip, Carrier, Vehicle, Invoice, and user queries predominantly use `AsNoTracking`.
- AutoMapper `ProjectTo` and explicit `Select` avoid loading complete entity graphs for read endpoints.
- Command paths use tracked aggregates and targeted includes.
- Notification read-all, refresh-token revocation, report-state changes, and carrier status changes use server-side bulk operations.
- Main and Payment APIs use `IMemoryCache` with a size limit of 130 and a five-minute expiration scan.
- Outbox processing uses a batch size of 20 and a maximum of five retries.

## Engineering Analysis

### Database Query Strengths

1. Stable pagination ordering uses `CreatedAt` plus `Id` for shipments.
2. Shipment indexes cover tracking number, status, sender, receiver, handler, trip, pickup and delivery timestamps, and common composites.
3. Trip indexes cover status, carrier, warehouse, creation date, and common composites.
4. Invoice indexes cover payment, reference, and customer IDs, time, status composites, and subscription-benefit reporting.
5. Optimistic concurrency avoids coarse locks on core operational aggregates.

### Critical EF Core Concurrency

The four dashboard builders start multiple async repository calls before awaiting them. Each repository is scoped and injects the same context. EF Core contexts are not thread-safe and reject overlapping operations. The correct options are:

- execute the queries sequentially on the scoped context; or
- inject `IDbContextFactory<AppDbContext>` into a dedicated dashboard read service and create one context per truly independent parallel query.

Sequential execution is the safer first correction. Optimize only after measuring dashboard latency and connection-pool impact.

### Search and Filtering

Shipment search constructs `%term%` patterns across tracking, owned-address columns, sender and receiver names, and city names. Leading wildcards generally prevent normal B-tree index seeks. For MVP volumes this is acceptable; at larger scale use full-text search, normalized searchable columns, or prefix search where product requirements allow.

The Admin filter runs a count and a page query, which is normal for page-number pagination. For very large tables, cursor pagination can remove high `Skip` costs. Current page sizes are small enough that page-number pagination is appropriate.

### Missing Indexes

The model snapshot does not define indexes on `Notifications`, even though reads filter by `UserId`, `IsRead`, and order by `CreatedOnUtc`. Add a composite index on `(UserId, IsRead, CreatedOnUtc DESC)`.

The outbox query filters `ProcessedOn IS NULL`, checks `RetryCount < 5`, orders by `OccuredAt`, and takes 20. Add a filtered or composite index aligned with that predicate and ordering. SQL Server filtered-index syntax should be verified in the generated migration.

### Caching

In-memory caching is fast and simple but each API replica owns a different cache. Cache invalidation only affects the instance handling the command. Before horizontal scale, use a distributed provider and include tenant or user scope in every key. Notifications correctly avoid caching.

### Background Work

Outbox messages are processed one at a time within a batch. That preserves simple failure isolation but limits throughput. Parallel publication should not be added until messages are atomically claimed and handlers are proven thread-safe.

QuestPDF includes large native runtime assets and report generation can consume significant CPU and memory. Hangfire should use a dedicated queue with a bounded worker count, maximum report payload size, and retention cleanup. The local Release build emitted retry warnings while copying QuestPDF native binaries because the workstation was resource constrained.

### External I/O

Payment calls are asynchronous and use typed settings. The mocked payment delay intentionally adds latency to Create Shipment. Production HTTP clients should have an explicit timeout, a retry policy limited to safe or idempotent operations, and circuit-breaker telemetry. Retrying a payment command without provider idempotency can double-charge, so policy must be operation-aware.

## Strengths

- Read-model projection avoids N+1 traversal in most endpoints.
- No lazy-loading proxy package is configured.
- `IQueryable` is kept inside repository implementation methods and is not returned by controllers.
- Bulk updates and deletes reduce round trips for set-based operations.
- Pagination and deterministic ordering are present on core lists.
- Operational tables already have many targeted indexes.

## Weaknesses

- Same-context dashboard parallelism is unsafe.
- Notification and outbox access paths lack matching indexes.
- Leading-wildcard multi-table search will degrade with volume.
- Process-local caching cannot remain coherent across replicas.
- Outbox processing has no database claim or lease for multi-instance execution.
- Report workload limits and queue metrics are not documented in code.

## Risks

| Priority | Bottleneck | Trigger |
| --- | --- | --- |
| High | EF concurrent-operation exception | Any real dashboard request with overlapping SQL |
| High | Outbox duplicate processing | More than one API or scheduler instance |
| Medium | Notification table scan | Large per-user notification history |
| Medium | Outbox queue scan | Large processed-message history |
| Medium | Shipment search scan | Growing shipment, address, and user tables |
| Medium | PDF memory pressure | Concurrent large report requests |
| Low | Offset pagination cost | Deep page numbers on large tables |

## Trade-offs

The current query approach is suitable for an MVP database. Full-text search, distributed cache, and cursor pagination should follow measured demand. The dashboard concurrency defect is different: it must be corrected regardless of current load.

## Metrics

| Metric | Value |
| --- | ---: |
| Outbox batch size | 20 |
| Outbox retry limit | 5 |
| Memory-cache size limit | 130 units |
| Default notification page size | 20 |
| Default shipment page size | 20 |
| Main API line coverage | 85.62% |
| Infrastructure line coverage | 80.14% |

## Production Readiness

Performance readiness is acceptable for a single-instance demo after correcting dashboard concurrency. Multi-instance production requires queue leasing, distributed cache, and index migrations.

## Recommendations

### High Priority

1. Correct dashboard context concurrency and add a SQL-backed regression test.
2. Add atomic outbox claiming before multiple API instances are allowed.
3. Add notification and outbox indexes with query-plan verification.

### Medium Priority

1. Add OpenTelemetry spans and duration histograms for dashboard queries, payment calls, outbox age, and report generation.
2. Bound Hangfire report workers and payload and file sizes.
3. Replace process-local cache before horizontal scaling.
4. Benchmark shipment search with production-like volume and choose full-text or normalized search columns if required.

### Low Priority

1. Consider cursor pagination for high-volume history endpoints.
2. Review duplicate and redundant indexes after collecting SQL Server usage statistics.

## Future Improvements

Create load tests for shipment filtering, dashboards, outbox backlog recovery, notification read-all, and concurrent bundle-benefit creation. Establish service-level objectives before selecting more infrastructure.

## Overall Score

**6.0/10**

## Final Verdict

The read-side foundations are good, but one release-blocking EF concurrency defect and several scale-path omissions prevent a higher production score.
