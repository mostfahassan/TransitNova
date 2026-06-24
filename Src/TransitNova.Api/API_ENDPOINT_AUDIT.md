# TransitNova API Endpoint Audit

## Executive summary

- Source reviewed: all ASP.NET Core controllers under `TransitNova.Api/Controllers`.
- Hosting model found: attribute-routed controllers; no Minimal API route mappings were found.
- In-scope endpoints: **97**.
- HTTP requests generated: **97** in `TransitNova.Api/HttpRequest.http`.
- Method distribution: **52 GET**, **14 POST**, **12 PUT**, **9 PATCH**, and **10 DELETE**.
- State-changing requests: **45**, all supplied with `X-Idempotency-Key`.
- Authenticated requests: **90**, all supplied with `Authorization: Bearer {{accessToken}}`.
- Public requests: registration, login, refresh-token operations, and public location lookups.
- Local sample URL: `https://localhost:7292`, taken from `Properties/launchSettings.json`.

## A. Endpoint inventory

### Authentication — `AuthenticationController` (4)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| POST | `/api/auth/register` | Anonymous; `X-Idempotency-Key` | `RegisterDto` body |
| POST | `/api/auth/login` | Anonymous; `X-Idempotency-Key` | `LoginDto` body |
| PUT | `/api/auth/change-password` | Bearer, all user roles; `X-Idempotency-Key` | `ChangePasswordDto` body |
| POST | `/api/auth/signout` | Bearer, all user roles; `X-Idempotency-Key` | No body |

### Tokens — `RefreshTokenController` (2)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| POST | `/api/v1/refresh-tokens` | Anonymous; `X-Idempotency-Key` | `RefreshToken` body (`token`) |
| DELETE | `/api/v1/refresh-tokens/{id}` | Anonymous; `X-Idempotency-Key` | Refresh-token GUID route value |

### Public locations — `CountryController`, `CityController` (3)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| GET | `/api/v1/countries` | Anonymous | None |
| GET | `/api/v1/countries/{countryId}/governments` | Anonymous | Integer country route value |
| GET | `/api/v1/governments/{governmentId}/cities` | Anonymous | Integer government route value |

### User subscriptions — `UserSubscriptionController` (2)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| POST | `/api/v1/subscriptions/bundles/{bundleId}/subscription` | Bearer, User + subscribe permission; `X-Idempotency-Key` | Bundle GUID route value |
| DELETE | `/api/v1/subscriptions/bundles/{bundleId}/subscription` | Bearer, User + unsubscribe permission; `X-Idempotency-Key` | Bundle GUID route value |

### User shipments — creation, operations, and query controllers (7)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| POST | `/api/v1/shipments` | Bearer, User; `X-Idempotency-Key` | `CreateShipmentDto` body |
| PUT | `/api/v1/shipments/{shipmentId}` | Bearer, User; `X-Idempotency-Key` | `UpdateShipmentDto` body |
| PATCH | `/api/v1/shipments/{shipmentId}` | Bearer, User; `X-Idempotency-Key` | No body |
| PATCH | `/api/v1/shipments/{shipmentId}/issue` | Bearer, User; `X-Idempotency-Key` | `IssueShipmentReason` body |
| DELETE | `/api/v1/shipments/{shipmentId}` | Bearer, User; `X-Idempotency-Key` | Shipment GUID route value |
| GET | `/api/v1/shipments/{trackingNumber}` | Bearer, User | Tracking-number route value |
| GET | `/api/v1/users/shipments/{shipmentId}` | Bearer, User | Shipment GUID route value |

### User profile — `ProfileController` (1)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| GET | `/api/v1/users/profile` | Bearer, User | None |

### User carrier ratings — `UserCarrierOperationController` (2)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| POST | `/api/v1/shipments/{shipmentId}/rate-pickup-carrier` | Bearer, User; `X-Idempotency-Key` | `RatingCarrierDto` body |
| POST | `/api/v1/shipments/{shipmentId}/rate-delivery-carrier` | Bearer, User; `X-Idempotency-Key` | `RatingCarrierDto` body |

### Carrier profile commands — `CarrierController` (2)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| PUT | `/api/v1/carriers/profile` | Bearer, Carrier; `X-Idempotency-Key` | `UpdateCarrierDto` body |
| PUT | `/api/v1/carriers/additional-info` | Bearer, Carrier; `X-Idempotency-Key` | `AdditionalInfoDto` body |

### Carrier operations — `CarrierOperationsController` (3)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| PATCH | `/api/v1/carriers/{carrierId}/status` | Bearer, Carrier; `X-Idempotency-Key` | `ChangeCarrierStatus` body |
| PATCH | `/api/v1/carriers/{carrierId}/shipments/{shipmentId}/complete-delivery` | Bearer, Carrier; `X-Idempotency-Key` | Two GUID route values |
| PATCH | `/api/v1/carriers/{carrierId}/shipments/{shipmentId}/complete-pickup` | Bearer, Carrier; `X-Idempotency-Key` | Two GUID route values |

### Carrier queries — `CarrierQueriesController` (5)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| GET | `/api/v1/carriers/{carrierId}/shipments` | Bearer, Carrier | `CarrierShipmentFilterDto` query |
| GET | `/api/v1/carriers/{carrierId}/shipments/{shipmentId}` | Bearer, Carrier | Two GUID route values |
| GET | `/api/v1/carriers/{carrierId}/profile` | Bearer, Carrier | Carrier GUID route value |
| GET | `/api/v1/carriers/{carrierId}/rating` | Bearer, Carrier | Carrier GUID route value |
| GET | `/api/v1/carriers/{carrierId}/revenue` | Bearer, Carrier | Carrier GUID route value |

### Carrier trips — `CarrierTripsController` (2)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| GET | `/api/v1/carriers/{carrierId}/trips` | Bearer, Carrier | Carrier GUID route value |
| GET | `/api/v1/carriers/{carrierId}/trips/{tripId}` | Bearer, Carrier | Carrier and trip GUID route values |

### Operation-manager trip commands — `OperationManagerTripOperationsController` (2)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| PATCH | `/api/v1/operation-managers/trips/{carrierId}/start-pickup` | Bearer, Operation Manager/Admin; `X-Idempotency-Key` | Carrier GUID route value |
| PATCH | `/api/v1/operation-managers/trips/{carrierId}/start-delivery` | Bearer, Operation Manager/Admin; `X-Idempotency-Key` | Carrier GUID route value |

### Operation-manager shipment queries — `OperationManagerShipmentsController` (6)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| GET | `/api/v1/operation-managers/shipments/assigned` | Bearer, Operation Manager/Admin | `ShipmentFilterDto` query |
| GET | `/api/v1/operation-managers/shipments/{shipmentId}/review` | Bearer, Operation Manager/Admin | Shipment GUID route value |
| GET | `/api/v1/operation-managers/shipments/{shipmentId}/histories` | Bearer, Operation Manager/Admin | Shipment GUID route value |
| GET | `/api/v1/operation-managers/shipments` | Bearer, Operation Manager/Admin | `ShipmentFilterDto` query |
| GET | `/api/v1/operation-managers/shipments/review-queue` | Bearer, Operation Manager/Admin | `ShipmentFilterDto` query |
| GET | `/api/v1/operation-managers/shipments/{shipmentId}` | Bearer, Operation Manager/Admin | Shipment GUID route value |

### Operation-manager shipment commands — `OperationManagerShipmentController` (2)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| PATCH | `/api/v1/operation-managers/shipments/{shipmentId}/approve` | Bearer, Operation Manager/Admin; `X-Idempotency-Key` | Shipment GUID route value |
| PATCH | `/api/v1/operation-managers/shipments/{shipmentId}/reject` | Bearer, Operation Manager/Admin; `X-Idempotency-Key` | `RejectShipmentReason` body |

### Operation-manager carrier queries — `OperationManagerCarrierController` (4)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| GET | `/api/v1/operation-managers/carriers` | Bearer, Operation Manager/Admin | `FilterCarrierDto` query |
| GET | `/api/v1/operation-managers/carriers/{carrierId}` | Bearer, Operation Manager/Admin | Carrier GUID route value |
| GET | `/api/v1/operation-managers/carriers/{carrierId}/shipments` | Bearer, Operation Manager/Admin | `CarrierShipmentFilterDto` query |
| GET | `/api/v1/operation-managers/carriers/{carrierId}/shipments/{shipmentId}` | Bearer, Operation Manager/Admin | Two GUID route values |

### Carrier assignment — `CarrierAssignmentController` (2)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| PUT | `/api/v1/operation-managers/carriers/{shipmentId}/assign-pickup` | Bearer, Operation Manager/Admin; `X-Idempotency-Key` | `AssignCarrierDto` body |
| PUT | `/api/v1/operation-managers/carriers/{shipmentId}/assign-delivery` | Bearer, Operation Manager/Admin; `X-Idempotency-Key` | `AssignCarrierDto` body |

### Operation-manager carrier trips — `OperationManagerCarrierTripOperationsController` (2)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| GET | `/api/v1/operation-managers/carriers/{carrierId}/trips` | Bearer, Operation Manager | Carrier GUID route value |
| GET | `/api/v1/operation-managers/carriers/{carrierId}/trips/{tripId}` | Bearer, Operation Manager | Carrier and trip GUID route values |

### Operation-manager details — `OperationManagersController` (3)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| GET | `/api/v1/operation-managers/{operationManagerId}` | Bearer, Operation Manager/Admin | Manager GUID route value |
| GET | `/api/v1/operation-managers/{operationManagerId}/handled-carriers` | Bearer, Operation Manager/Admin | `pageNumber`, `pageSize` query |
| GET | `/api/v1/operation-managers/{operationManagerId}/handled-shipments` | Bearer, Operation Manager/Admin | `pageNumber`, `pageSize` query |

### Admin warehouses — `WarehouseController` (5)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| POST | `/api/v1/admin/warehouses` | Bearer, Admin; `X-Idempotency-Key` | `CreateWarehouseDto` body |
| PUT | `/api/v1/admin/warehouses/{warehouseId}` | Bearer, Admin; `X-Idempotency-Key` | `UpdateWarehouseDto` body |
| DELETE | `/api/v1/admin/warehouses/{warehouseId}` | Bearer, Admin; `X-Idempotency-Key` | Warehouse GUID route value |
| GET | `/api/v1/admin/warehouses` | Bearer, Admin | None |
| GET | `/api/v1/admin/warehouses/{warehouseId}` | Bearer, Admin | Warehouse GUID route value |

### Admin roles — `RolesController` (7)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| GET | `/api/v1/admin/roles` | Bearer, Admin | None |
| GET | `/api/v1/admin/roles/{roleId}` | Bearer, Admin | Role GUID route value |
| GET | `/api/v1/admin/roles/{roleId}/members` | Bearer, Admin | Role GUID route value |
| POST | `/api/v1/admin/roles` | Bearer, Admin; `X-Idempotency-Key` | `RoleNameDto` body |
| PUT | `/api/v1/admin/roles/{roleId}` | Bearer, Admin; `X-Idempotency-Key` | `RoleNameDto` body |
| DELETE | `/api/v1/admin/roles/{roleId}` | Bearer, Admin; `X-Idempotency-Key` | Role GUID route value |
| PUT | `/api/v1/admin/roles/{roleId}/members` | Bearer, Admin; `X-Idempotency-Key` | `UpdateRoleMembersDto` body |

### Admin bundles — `BundlesController` (5)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| POST | `/api/v1/admin/bundles` | Bearer, Admin; `X-Idempotency-Key` | `CreateBundleDto` body |
| PUT | `/api/v1/admin/bundles` | Bearer, Admin; `X-Idempotency-Key` | `UpdateBundleDto` body |
| DELETE | `/api/v1/admin/bundles/{bundleId}` | Bearer, Admin; `X-Idempotency-Key` | Bundle GUID route value |
| GET | `/api/v1/admin/bundles` | Bearer, Admin | None |
| GET | `/api/v1/admin/bundles/{bundleId}` | Bearer, Admin | Bundle GUID route value |

### Admin cities — `CitiesController` (5)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| POST | `/api/v1/admin/cities` | Bearer, Admin; `X-Idempotency-Key` | `CreateCityDto` body |
| PUT | `/api/v1/admin/cities/{cityId}` | Bearer, Admin; `X-Idempotency-Key` | `UpdateCityDto` body |
| DELETE | `/api/v1/admin/cities/{cityId}` | Bearer, Admin; `X-Idempotency-Key` | Integer city route value |
| GET | `/api/v1/admin/cities/{cityId}` | Bearer, Admin | Integer city route value |
| GET | `/api/v1/admin/cities` | Bearer, Admin | `CityFilterDto` query |

### Admin governments — `GovernmentsController` (5)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| POST | `/api/v1/admin/governments` | Bearer, Admin; `X-Idempotency-Key` | `CreateGovernmentDto` body |
| PUT | `/api/v1/admin/governments/{governmentId}` | Bearer, Admin; `X-Idempotency-Key` | `UpdateGovernmentDto` body |
| DELETE | `/api/v1/admin/governments/{governmentId}` | Bearer, Admin; `X-Idempotency-Key` | Integer government route value |
| GET | `/api/v1/admin/governments/{governmentId}` | Bearer, Admin | Integer government route value |
| GET | `/api/v1/admin/governments` | Bearer, Admin | None |

### Admin vehicles — `VehicleController` (6)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| POST | `/api/v1/admin/vehicles` | Bearer, Admin; `X-Idempotency-Key` | `VehicleDto` body |
| DELETE | `/api/v1/admin/vehicles/{vehicleId}` | Bearer, Admin; `X-Idempotency-Key` | Vehicle GUID route value |
| GET | `/api/v1/admin/vehicles` | Bearer, Admin | None |
| GET | `/api/v1/admin/vehicles/active` | Bearer, Admin | None |
| GET | `/api/v1/admin/vehicles/{vehicleId}` | Bearer, Admin | Vehicle GUID route value |
| GET | `/api/v1/admin/vehicles/plate-number/{plateNumber}` | Bearer, Admin | Plate-number route value |

### Admin carrier operations — `AdminsCarrierOperationController` (1)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| DELETE | `/api/v1/admin/carriers/{id}` | Bearer, Admin; `X-Idempotency-Key` | Carrier GUID route value |

### Admin users — `AdminUsersController` (2)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| GET | `/api/v1/admin/users` | Bearer, Admin | `UserFiltrationDto` query |
| GET | `/api/v1/admin/users/{userId}` | Bearer, Admin | User GUID route value |

### Admin subscriptions — `SubscriptionsController` (2)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| GET | `/api/v1/admin/subscriptions/{subscriptionId}` | Bearer, Admin | Subscription GUID route value |
| GET | `/api/v1/admin/subscriptions/bundles/{bundleId}/subscribers` | Bearer, Admin | Bundle GUID route value |

### Admin operation managers — `OperationManagersController` (5)

| Method | Route | Authentication | Request contract |
|---|---|---|---|
| GET | `/api/v1/admin/operation-managers` | Bearer, Admin | None |
| GET | `/api/v1/admin/operation-managers/active` | Bearer, Admin | None |
| GET | `/api/v1/admin/operation-managers/{operationManagerId}` | Bearer, Admin | Manager GUID route value |
| GET | `/api/v1/admin/operation-managers/{operationManagerId}/handled-carriers` | Bearer, Admin | `pageNumber`, `pageSize` query |
| GET | `/api/v1/admin/operation-managers/{operationManagerId}/handled-shipments` | Bearer, Admin | `pageNumber`, `pageSize` query |

## B. Coverage check

| Check | Result |
|---|---:|
| Eligible controller actions discovered | 97 |
| Corresponding `.http` requests | 97 |
| Missing requests | 0 |
| Duplicate request sections | 0 |
| Commands requiring idempotency headers | 45 |
| Commands supplied with idempotency headers | 45 |
| Requests requiring bearer authentication | 90 |
| Requests supplied with bearer authentication | 90 |

Every in-scope controller action has one independently runnable request section. No Minimal API endpoints were found. The suite deliberately uses reusable variables for identifiers and credentials, so a QA operator can replace persisted IDs and the JWT once and run individual requests.

## C. Request quality check

### DTO and JSON accuracy

- JSON property names match the actual DTO properties under the ASP.NET Core camel-case JSON policy.
- Enum examples use names such as `User`, `EGP`, `Land`, `Express`, `Available`, `Truck`, and `MainWarehouse`, matching the configured `JsonStringEnumConverter`.
- Nullable properties are represented only where useful and accepted by the corresponding DTO.
- Nested shipment data mirrors `CreateReceiverDto` and `PackageSpecificationDto` exactly.
- Array query binding is demonstrated with repeated `status` and `servedZones` keys, which is compatible with ASP.NET Core model binding.
- All sample dates use valid ISO 8601 values.

### Headers

- All JSON bodies include `Content-Type: application/json`.
- All requests include `Accept: application/json`.
- Every state-changing action includes a generated GUID via `X-Idempotency-Key: {{$guid}}`.
- Every action protected by controller or method authorization includes `Authorization: Bearer {{accessToken}}`.

### Runtime prerequisites

The requests are syntactically runnable, but successful business responses require:

1. A locally trusted ASP.NET Core development certificate and the API running on port `7292`.
2. A valid JWT with the role and permission required by the selected action.
3. Database records matching the sample GUID/integer variables.
4. Shipment, carrier, trip, and subscription records in business-valid states for transition commands.
5. A refresh token issued by this API for refresh-token rotation.

No required DTO field was omitted from the provided create/update examples. Fields not accepted by a given DTO were not invented.

## Resolved consistency issues

- Route parameters are now the single source of truth for shipment, city, government, role-membership, and carrier-status updates. Their request DTOs no longer repeat those identifiers.
- Public route segments now use lowercase plural resource names: `countries`, `governments`, `users`, `carriers`, `subscriptions`, `refresh-tokens`, and `operation-managers`.
- Carrier metrics now use lowercase `/rating` and `/revenue` paths.
- Public location endpoints are versioned consistently under `/api/v1`.
- The country-to-governments route now names its identifier `countryId`.
- The tracking action method and carrier delivery path now match their actual intent and route structure.

## D. Risks and issues

### High priority

1. **Refresh-token revocation has no authorization attribute.** `DELETE /api/v1/refresh-tokens/{id}` accepts a token-record GUID and is publicly reachable according to the controller. If token identifiers leak or can be inferred, another party may attempt to revoke sessions. Authorization or proof-of-possession should be reviewed.

### Medium priority

2. **Create-vehicle uses a shared `VehicleDto`.** The request still includes identity and calculated-looking carrier rating data alongside writable vehicle fields. A dedicated create request DTO would make field ownership explicit.
3. **Carrier-list actions combine two separate authorization policies.** The operation-manager carrier query controller applies both admin and operation-manager policies to the same actions. Multiple `[Authorize]` attributes are cumulative, so access can fail unless one principal satisfies both policy sets.
4. **Subscription actions apply a shipment-owner policy at controller level.** These routes contain a bundle identifier rather than a shipment identifier, so resource-policy resolution should be verified to avoid unexpected denials.

### Low priority

5. **Bundle update is collection-root based.** `PUT /api/v1/admin/bundles` carries `bundleId` only in the body, unlike most update routes. This is valid but inconsistent with the rest of the API.
6. **Cancellation uses a generic PATCH route with no body.** `PATCH /shipments/{shipmentId}` does not communicate the action as explicitly as the neighboring `/issue`, `/approve`, and `/reject` routes.

## E. Completeness score

| Dimension | Score | Rationale |
|---|---:|---|
| Coverage completeness | **100%** | 97 of 97 in-scope controller actions have exactly one request. |
| Request accuracy | **99%** | DTOs, enums, headers, paths, and query binding match source; successful execution still depends on live identities, records, permissions, and business state. |
| API consistency | **96%** | Resource casing, plurality, API versioning, route naming, and identifier ownership are now consistent; a few broader authorization and request-model concerns remain. |

## QA usage notes

1. Start `TransitNova.Api` with the HTTPS launch profile.
2. Run registration/login first or paste an issued JWT into `@accessToken`.
3. Replace shared IDs at the top of `HttpRequest.http` with records from the test database.
4. Run prerequisite create/approve/assign operations before state-transition requests.
5. Keep a new idempotency key per intentional operation; the file generates one automatically for every command execution.
