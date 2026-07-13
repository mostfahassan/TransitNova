# API Design Review

## Purpose

Evaluate REST design, versioning, route naming, response consistency, status codes, filtering, sorting, pagination, serialization, OpenAPI, idempotency, and the Admin Shipment surface.

## Scope

The Main API public contract represented by the verified OpenAPI document, controller metadata, result conversion, ProblemDetails, UI deserialization, and the Payment API trust boundary.

## Executive Summary

The Main API exposes a large and well-organized v1 surface: 121 paths and 142 operations. Routes are resource-oriented, role-specific scopes are visible, command endpoints use idempotency, and OpenAPI metadata includes names, summaries, descriptions, and status declarations. DTO projection, string-enum JSON, and a tested UI response reader prevent the serialization failures common in EF-backed applications.

The main contract weaknesses are inconsistent unexpected and validation status mapping, weak response schema declarations for `IActionResult`, unbounded query page size, several hardcoded UI filter options, and a date-range implementation that compares different shipment timestamps. Admin Shipment read pages work through list and details endpoints, but their generic reflection model and missing pagination controls reduce reliability.

## Current Implementation

### Contract Inventory

| HTTP method | Operations |
| --- | ---: |
| GET | 87 |
| POST | 19 |
| PUT | 13 |
| PATCH | 12 |
| DELETE | 10 |
| **Total** | **141** |

The verified OpenAPI document is version 3.1.1. The API uses URL-segment versioning through `/api/v{version:apiVersion}` and defaults to v1 when unspecified.

### Response Envelope

Successful result responses serialize to a stable shape:

```json
{
  "isSuccess": true,
  "message": "Operation completed.",
  "statusCode": 200,
  "data": {}
}
```

Expected errors use:

```json
{
  "isSuccess": false,
  "statusCode": 404,
  "errorCode": "NOT_FOUND",
  "message": "The requested resource was not found."
}
```

Validation errors use an `errors` array. Unhandled exceptions use RFC-style ProblemDetails. `HttpHandler` can deserialize all these paths and creates a structured failure when JSON is invalid or the body is empty.

### Serialization

- System.Text.Json uses web defaults and `JsonStringEnumConverter` in Main API, Payment API, UI MVC, and outbound payment clients.
- Public endpoints return DTOs or anonymous response envelopes rather than EF entities.
- Repository `IQueryable` objects remain implementation-private.
- Lazy-loading proxies are not enabled.
- No `ReferenceHandler.IgnoreCycles` workaround is configured because DTO projection avoids entity cycles.
- `ApiResponse` and `Result` types have explicit JSON constructors for immutable properties.

## Engineering Analysis

### Status Mapping

`ResultExtensions` maps Created to 201, success to 200, NotFound to 404, Unauthorized to 401, Forbidden to 403, Conflict to 409, and ValidationError to 422. An unsuccessful `ResultStatus.UnExpected` falls into the default 400 branch. It must map to 500.

`ResultStatus.NoContent` creates an ObjectResult with status 204 and an envelope body. HTTP 204 does not permit a response body, so clients should receive a true empty response or the endpoint should return 200 with an envelope.

FluentValidation failures returned by the MediatR behavior become 422, while a thrown FluentValidation exception becomes 400 in the global handler. Pick one contract and test it consistently.

### Response Schema Documentation

Controllers return `IActionResult` and often declare only status codes in `ProducesResponseType`. OpenAPI can describe the route but may not expose the exact generic data schema. Prefer `ActionResult<ApiEnvelope<T>>`, typed results, or `ProducesResponseType(typeof(ApiEnvelope<T>), 200)` for public response types.

### Filtering and Pagination

Shipment filter supports multiple statuses, transportation mode, From, To, sender, warehouse scope, search term, page number, and page size. Search is passed correctly from MVC ViewModel through UI DTO and API query. Page values below one are normalized, but there is no maximum page size in the repository path. Add validation and cap it, such as 100.

The shipment date filter uses `PickupDate >= From` and `ActualDeliveryDate <= To`, mixing two business events. Define whether the range means CreatedAt, pickup window, or actual delivery and use one field for both boundaries.

Sorting is fixed for many endpoints. Where users need comparison, expose a whitelist enum for sort field and direction rather than accepting raw property names.

### Admin Shipment Area

Implemented API routes:

- `GET /api/v1/admin/shipments`
- `GET /api/v1/admin/shipments/{shipmentId}`

Implemented MVC actions and views:

- `AdminArea/Shipments/Index`
- `AdminArea/Shipments/Details/{shipmentId}`

The table action correctly passes `shipmentId` to Details. The typed UI client expects `UiPagedResult<UiRetrieveShipmentDto>` for list and `UiRetrieveShipmentDto` for details. DTO names align with backend fields, including nested addresses, sender, receiver, package, states, status, mode, type, cost, and dates.

Remaining UI contract gaps:

- `_AdminTable` displays page and total-page metadata but renders no previous, next, or page links.
- Status options are hardcoded and omit valid states such as Rejected, Cancelled, OutForPickup, PickedUp, OutForDelivery, Issue, and Deleted.
- The status select does not restore selected options after filtering.
- Mode and sender filters exist in the contract but are not exposed by the Admin view.
- `@model object?` and reflection property paths allow silent display failure after DTO changes.
- API summary text for Admin list incorrectly says Operation Manager.

### Idempotency

Command endpoints bind an `X-Idempotency-Key` value to a GUID request ID. The pipeline stores a hash of the serialized request and the serialized response. Reusing a key with different content returns a conflict. This is a strong design, but records need retention and response schema versioning.

### OpenAPI and Documentation

OpenAPI, Swagger UI, and Scalar are available only in Development or Testing for Main API, which is appropriate. The verified snapshot catches route and schema drift. Security schemes and operation requirements are added by document and operation transformers.

## Strengths

- Resource-oriented versioned routes and role-specific route groups.
- Rich endpoint metadata and a verified OpenAPI snapshot.
- No EF entity exposure or circular-reference workaround.
- String enums are consistent across server and clients.
- UI deserialization handles envelopes and ProblemDetails robustly.
- Idempotency and rate limiting protect commands.
- Ownership comes from claims or resource authorization rather than client-supplied user IDs on shared endpoints.

## Weaknesses

- Unexpected failed results map to 400 instead of 500.
- Validation status semantics are split between 400 and 422.
- Response schemas are not always explicit in OpenAPI.
- Page size is not consistently capped.
- Admin Shipment filter and pagination UI are incomplete.
- Payment API uses a separate result contract and static authentication header.

## Risks

- Clients can treat server errors as correctable bad requests.
- A large PageSize can create expensive materialization and payloads.
- Generic Razor property paths can hide DTO drift until manual testing.
- Date filtering can return surprising or missing records.
- Contract duplication can diverge between Main, UI, and Payment.

## Trade-offs

Role-specific routes improve authorization clarity at the cost of repeated controller surfaces. That is acceptable for this operational product. The response envelope is also appropriate for MVC client consumption, provided ProblemDetails and status mapping are normalized.

## Metrics

| Metric | Value |
| --- | ---: |
| OpenAPI paths | 120 |
| OpenAPI operations | 141 |
| API version | v1 |
| Main API controllers | 49 |
| Read operations | 87 |
| State-changing operations | 54 |

## Production Readiness

The API is demo-ready and structurally strong. Status consistency, page limits, typed response schemas, and high-value UI contract testing are needed for production.

## Recommendations

### High Priority

1. Add an explicit exhaustive `ResultStatus` to HTTP mapping and test every value.
2. Cap and validate every paged endpoint.
3. Define shipment date-filter semantics and correct repository predicates.
4. Add browser tests for Admin Shipment list, filters, pagination, and details action.

### Medium Priority

1. Publish explicit response schemas in OpenAPI.
2. Generate the UI client from OpenAPI or extract a dedicated Contracts assembly.
3. Render shipment status and mode options from shared enum values and preserve selection.
4. Add pagination links while retaining the current query string.

### Low Priority

1. Correct inaccurate endpoint summaries and naming defects.
2. Add deprecation and version-lifecycle guidance before v2.

## Future Improvements

Add consumer-driven contract tests, standardized cursor pagination for high-volume history, and correlation IDs in every documented error response.

## Overall Score

**7.6/10**

## Final Verdict

The API is one of the strongest parts of the project. Its remaining issues are contract consistency and UI completeness, not fundamental REST or serialization design failures.

