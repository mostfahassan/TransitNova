# Cache And Controller Review

Date: 2026-06-28

## Scope
- Projects reviewed: `Src/TransitNova.Api`, `Src/TransitNova.BusinessLayer`, `Src/TransitNova.Domain`, `Src/TransitNova.InfraStructure`, `TransitNovaPayment/TransitNovaPayment.API`, `TransitNovaPayment/TransitNovaPayment.Busieness`, `TransitNovaPayment/TransitNovaPayment.Infrastructure`, and the UI controller surface for endpoint inventory.
- Endpoints reviewed count: `116`
- Commands reviewed count: `60`
- Queries reviewed count: `61`

## Audit Summary
- Confirmed timing pattern: reviewed handlers that call `RemoveAsync` do so after `SaveChangesAsync` or after a positive repository result. I did not find a confirmed case where an obvious failed persistence path still invalidates cache.
- Main gap: related list, filter, paged, and dashboard caches are frequently not invalidated at all.
- Mutation logic was intentionally not changed in this pass. Only missing cache-key registrations were added to `Src/TransitNova.InfraStructure/Common/CacheService/MemoryCacheService.cs`.

## 1. Cache Defects Found

CACHE DEFECT
Feature: Admin dashboard aggregate cache
File: `Src/TransitNova.BusinessLayer/Features/Admin/Queries/GetAdminDashboardQuery.cs`
Location: `GetAdminDashboardQuery.CacheKey` and the mutation handlers reviewed across the solution
Missing Cache Key: `CacheKeys.AdminDashboard()`
Reason: `GetAdminDashboardQuery` is cached, but no mutation handler removes this key even though the dashboard aggregates shipments, carriers, users, operation managers, trips, and ratings.
Recommended Fix: Add a shared post-commit admin-dashboard invalidation hook that all KPI-changing mutations can call.

CACHE DEFECT
Feature: Carrier filter cache
File: `Src/TransitNova.BusinessLayer/Features/Carriers/Queries/Carrier/FilterCarriersQuery.cs`
Location: `AddCarrierAdditionalInfoCommandHandler.Handle`, `UpdateCarrierProfileHandler.Handle`, `UpdateCarrierStatusCommandHandler.Handle`, `DeleteCarrierHandler.Handle`, `RatingPickupCarrierHandler.Handle`, `RatingDeliveryCarrierHandler.Handle`, `CreateVehicleHandler.Handle`, `UpdateVehicleHandler.Handle`, `DeleteVehicleHandler.Handle`
Missing Cache Key: `CacheKeys.CarrierFilter(filterCriteria)`
Reason: `FilterCarriersQuery` caches `CarrierProfileDto`, which contains profile fields, status, rating, and vehicle data. None of the related mutations remove filtered carrier caches, so list pages can stay stale indefinitely.
Recommended Fix: Add carrier-prefix invalidation after successful persistence. Vehicle reassignment must invalidate both old and new carrier ownership views.

CACHE DEFECT
Feature: Carrier shipment list cache
File: `Src/TransitNova.BusinessLayer/Features/Carriers/Queries/Shipment/GetCarrierShipmentsQuery.cs`
Location: `AssignShipmentDeliveryToCarrierHandler.Handle`, `AssignShipmentPickupToCarrierHandler.Handle`, `CompleteShipmentCommandHandler.Handle`, `CompleteShipmentToWarehouseHandler.Handle`, `StartDeliveryTripHandler.Handle`, `StartPickupTripHandler.Handle`
Missing Cache Key: `CacheKeys.CarrierShipments(carrierId, filter)`
Reason: cached carrier shipment lists are never invalidated anywhere in the solution, even though assignment, trip start, pickup, warehouse delivery, and final delivery all change list membership and row status.
Recommended Fix: Add post-commit carrier-shipment invalidation keyed by carrier id and invalidate both previous and new carrier scopes on reassignment.

CACHE DEFECT
Feature: Operation manager handled pages
File: `Src/TransitNova.BusinessLayer/Features/OperationManagerService/Queries/OperationManagerQueries/GetOperationManagerHandledCarriersQuery.cs`, `Src/TransitNova.BusinessLayer/Features/OperationManagerService/Queries/OperationManagerQueries/GetOperationManagerHandledShipmentsQuery.cs`
Location: `ApproveShipmentHandler.Handle`, `RejectShipmentHandler.Handle`, `AssignShipmentDeliveryToCarrierHandler.Handle`, `AssignShipmentPickupToCarrierHandler.Handle`, and later shipment lifecycle mutations
Missing Cache Key: `CacheKeys.OperationManagerHandledCarriers(operationManagerId, pageNumber, pageSize)`, `CacheKeys.OperationManagerHandledShipments(operationManagerId, pageNumber, pageSize)`
Reason: both paged queries are cached, but there is no invalidation path anywhere in the solution. Approval, rejection, assignment, and downstream shipment state changes all alter these datasets.
Recommended Fix: Add prefix-based invalidation for handled-carrier and handled-shipment page caches after commit.

CACHE DEFECT
Feature: Bundle subscription deactivation
File: `Src/TransitNova.BusinessLayer/Features/UserOperations/Handlers/CommandsHandler/Bundles/UnsubscribeFromBundleHandler.cs`
Location: `UnsubscribeFromBundleHandler.Handle`
Missing Cache Key: `CacheKeys.BundleSubscriptionDetails(activeSubscription.Id)`
Reason: `GetBundleSubscriptionDetailsQuery` caches `IsActive`, `EndDate`, and `CancelledAt`. Unsubscribe mutates exactly those fields but only clears user and bundle list caches.
Recommended Fix: Invalidate the active subscription details cache after successful `SaveChangesAsync`.

CACHE DEFECT
Feature: City list filters
File: `Src/TransitNova.BusinessLayer/Features/Location/Cities/Queries/FilterCitiesQuery.cs`
Location: `CreateCityHandler.Handle`, `UpdateCityHandler.Handle`, `DeleteCityHandler.Handle`
Missing Cache Key: `CacheKeys.CityFilter(filter)`
Reason: city create, update, and delete clear `CitiesByGovernment` and `CityById`, but never clear the cached filtered city pages.
Recommended Fix: Add city-filter invalidation after successful persistence.

CACHE DEFECT
Feature: Zone list filters
File: `Src/TransitNova.BusinessLayer/Features/Zones/Queries/FilterZonesQuery.cs`
Location: `CreateZoneHandler.Handle`, `UpdateZoneHandler.Handle`, `DeleteZoneHandler.Handle`
Missing Cache Key: `CacheKeys.ZoneFilter(filter)`
Reason: zone mutations do not touch the cache service at all, while `FilterZonesQuery` is cached.
Recommended Fix: Add zone-filter invalidation after successful persistence.

CACHE DEFECT
Feature: Trip list and detail caches
File: `Src/TransitNova.BusinessLayer/Features/OperationManagerService/Queries/Trips/FilterTripsQuery.cs`, `Src/TransitNova.BusinessLayer/Features/WarehouseManagers/Queries/GetTripDetailsQuery.cs`
Location: `StartPickupTripHandler.Handle`, `StartDeliveryTripHandler.Handle`, `CancelTripHandler.Handle`, `UpdateTripHandler.Handle`
Missing Cache Key: `CacheKeys.TripFilter(filter)`, `CacheKeys.TripDetails(tripId)`
Reason: trip start handlers never clear cached trip filters, cancel and update only clear the default `new FilterTripsDto()` key, and `UpdateTripHandler` forgets `TripDetails(tripId)` when the carrier stays the same.
Recommended Fix: Invalidate trip-filter caches by prefix, and always clear `TripDetails(tripId)` after any successful trip update.

CACHE DEFECT
Feature: Shipment sender-facing caches
File: `Src/TransitNova.BusinessLayer/Features/UserOperations/Queries/GetUserDashboardQuery.cs`, `Src/TransitNova.BusinessLayer/Features/UserOperations/Queries/GetUserShipmentQuery.cs`
Location: `AssignShipmentDeliveryToCarrierHandler.Handle`, `AssignShipmentPickupToCarrierHandler.Handle`, `ApproveShipmentHandler.Handle`, `RejectShipmentHandler.Handle`, `CompleteShipmentCommandHandler.Handle`, `CompleteShipmentToWarehouseHandler.Handle`
Missing Cache Key: `CacheKeys.UserDashboard(senderId)`, `CacheKeys.UserShipment(senderId, shipmentId)`
Reason: sender-visible shipment status changes are cached, but several non-user mutations only clear tracking and operation-manager caches. Sender dashboard and detail views can therefore serve stale shipment status data.
Recommended Fix: Invalidate sender-specific caches using the mutated shipment's `SenderId` after persistence succeeds.

CACHE DEFECT
Feature: Warehouse manager dashboard cache
File: `Src/TransitNova.BusinessLayer/Features/WarehouseManagers/Queries/GetWarehouseManagerDashboardQuery.cs`, `Src/TransitNova.BusinessLayer/Features/WarehouseManagers/Handlers/ApplyCommands/UpdateWarehouseManagerHandler.cs`
Location: `GetWarehouseManagerDashboardQuery.CacheKey`, `UpdateWarehouseManagerHandler.Handle`
Missing Cache Key: `warehouse-managers:dashboard:manager-id:{ManagerId}`
Reason: the dashboard key is hardcoded outside `CacheKeys`, is cached, and is not invalidated even when the warehouse manager profile is updated. The same key is also invisible to most shipment and trip mutations that change dashboard KPIs.
Recommended Fix: Move the key into `CacheKeys`, add a dedicated invalidation helper, and clear it after warehouse-manager profile and KPI-driving mutations.

CACHE DEFECT
Feature: Pickup carrier reassignment
File: `Src/TransitNova.BusinessLayer/Services/ShipmentAssignmentServices/ShipmentAssignmentService.cs`, `Src/TransitNova.BusinessLayer/Features/OperationManagerService/Handlers/Commands/Carriers/AssignShipmentPickupToCarrierHandler.cs`
Location: `AssignPickupAsync` and `AssignShipmentPickupToCarrierHandler.Handle`
Missing Cache Key: old carrier `CarrierShipmentDetails`, `CarrierShipments`, `CarrierTrips`, `CarrierDashboard`
Reason: pickup assignment loads shipments already in `AssignedToPickUpCarrier` status, so the flow can overwrite an existing carrier assignment. The handler only invalidates the new carrier scope and never the previous carrier scope.
Recommended Fix: Capture the previous carrier id before reassignment and invalidate both old and new carrier caches after commit.

## 2. Cache Defects Fixed
- None. Per the audit-first instruction, mutation invalidation logic was left unchanged and reported instead of being silently altered.

## 3. Missing Cache Keys Added
- File updated: `Src/TransitNova.InfraStructure/Common/CacheService/MemoryCacheService.cs`
- Added cache-service registrations for active keys that were previously falling through the default branch:
  - `CacheKeys.AdminDashboard()`
  - `CacheKeys.BundleSubscriptionDetails(subscriptionId)`
  - `CacheKeys.CarrierProfile(carrierId)`
  - `CacheKeys.CarrierShipmentDetails(carrierId, shipmentId)`
  - `CacheKeys.CarrierTripDetails(carrierId, tripId)`
  - `CacheKeys.CityById(id)`
  - `CacheKeys.OperationManagerDetails(operationManagerId)`
  - `CacheKeys.ShipmentByTrackingNumber(trackingNumber)`
  - `CacheKeys.TripFilter(filter)`
  - `CacheKeys.TripDetails(tripId)`
  - `CacheKeys.AdminUserDetails(userId)`
  - `CacheKeys.UserProfile(appUserId)`
  - `CacheKeys.UserShipment(appUserId, shipmentId)`
  - `CacheKeys.VehicleList()`
  - `warehouse-managers:dashboard:manager-id:{ManagerId}`

## 4. Dead Cache Keys Detected
A cache key was treated as dead when no live `ICachable` query can populate it.

- Main solution:
  - `CacheKeys.BundleById`
  - `CacheKeys.CarrierRating`
  - `CacheKeys.CarriersByStatus`
  - `CacheKeys.CarrierShipmentsByCarrier`
  - `CacheKeys.CitiesByCountry`
  - `CacheKeys.CountryById`
  - `CacheKeys.CountryList`
  - `CacheKeys.CountryGovernments`
  - `CacheKeys.CountryFilter`
  - `CacheKeys.OperationManagerAssignedShipments`
  - `CacheKeys.OperationManagerFilterCarriers`
  - `CacheKeys.ShipmentById`
  - `CacheKeys.ShipmentFilter`
  - `CacheKeys.ShipmentHistories`
  - `CacheKeys.ShipmentStatistics`
  - `CacheKeys.UserShipmentByTrackingNumber`
  - `CacheKeys.UserShipmentsInfo`
  - `CacheKeys.UserId`
  - `CacheKeys.ActiveVehicles`
  - `CacheKeys.VehicleById`
  - `CacheKeys.VehicleByPlateNumber`
  - `CacheKeys.VehiclesByCarrierId`
  - `CacheKeys.ZoneById`
  - `CacheKeys.ZonesByCity`
- Payment solution:
  - None detected.

## 5. Controller Inconsistencies Found

CONTROLLER CONSISTENCY ISSUE
Controller: `AuthenticationController`
Endpoint: `POST /api/auth/register`, `POST /api/auth/login`, `PUT /api/auth/change-password`, `POST /api/auth/signout`
File: `Src/TransitNova.Api/Controllers/Account/AuthenticationController.cs`
Current Behavior: Uses the unversioned route base `api/auth` and has no `[ApiVersion]` attribute while the rest of the main API is versioned under `api/v{version:apiVersion}/...`.

Expected Behavior: Use the same API-versioning convention as the rest of the main API.
Reason: Clients have to special-case authentication endpoints, which weakens route consistency and version negotiation.

CONTROLLER CONSISTENCY ISSUE
Controller: `PaymentController`
Endpoint: `POST /api/payments/pay`, `POST /api/payments/history`
File: `TransitNovaPayment/TransitNovaPayment.API/Controllers/Payment/PaymentController.cs`
Current Behavior: Uses the unversioned route base `api/payments` and no `[ApiVersion]` attribute.
Expected Behavior: Use the same versioned route convention as the rest of the solution, or clearly isolate the payment API under a documented separate contract.
Reason: The payment API currently diverges from the main versioning strategy.

CONTROLLER CONSISTENCY ISSUE
Controller: `PaymentController`
Endpoint: `POST /api/payments/history`
File: `TransitNovaPayment/TransitNovaPayment.API/Controllers/Payment/PaymentController.cs`
Current Behavior: A read-only history query is exposed as `POST` and uses the command limiter.
Expected Behavior: Expose retrieval as `GET` or explicitly document the exception and apply the default/query limiter.
Reason: The endpoint does not mutate state, so its verb and limiter diverge from the rest of the solution's query conventions.

CONTROLLER CONSISTENCY ISSUE
Controller: `PaymentController`
Endpoint: `POST /api/payments/pay`
File: `TransitNovaPayment/TransitNovaPayment.API/Controllers/Payment/PaymentController.cs`
Current Behavior: Payment creation is guarded by `X-PaymentKey` only; no idempotency key or replay protection is used.
Expected Behavior: Use idempotency for payment creation, matching the main API payment endpoint.
Reason: Payment submission is the kind of retry-prone mutation that most needs replay protection.

CONTROLLER CONSISTENCY ISSUE
Controller: `CityController`
Endpoint: `GET /api/v{version:apiVersion}/governments/{governmentId:int}/cities`
File: `Src/TransitNova.Api/Controllers/Locations/City/CityController.cs`
Current Behavior: Returns `Ok(result.Data)` directly and omits `ProducesResponseType`, `EndpointName`, `EndpointSummary`, `EndpointDescription`, and `Tags` metadata.
Expected Behavior: Follow the same standardized result mapping and endpoint metadata pattern used by the rest of the API.
Reason: Public location endpoints are the outliers in both documentation and response-contract shape.

CONTROLLER CONSISTENCY ISSUE
Controller: `CountryController`
Endpoint: `GET /api/v{version:apiVersion}/countries`, `GET /api/v{version:apiVersion}/countries/{countryId:int}/governments`
File: `Src/TransitNova.Api/Controllers/Locations/Country/CountryController.cs`
Current Behavior: Returns `Ok(result.Data)` directly and omits `ProducesResponseType`, `EndpointName`, `EndpointSummary`, `EndpointDescription`, and `Tags` metadata.
Expected Behavior: Follow the same standardized result mapping and endpoint metadata pattern used by the rest of the API.
Reason: These endpoints bypass the result-envelope convention that the rest of the main API uses.

CONTROLLER CONSISTENCY ISSUE
Controller: `AdminsController`
Endpoint: `GET /api/v{version:apiVersion}/admins/dashboard`
File: `Src/TransitNova.Api/Controllers/Admin/AdminDashboard/AdminsController.cs`
Current Behavior: Uses plural `/admins/dashboard` while the rest of the admin area uses the singular `/admin/...` base route.
Expected Behavior: Align on one admin base route, preferably `api/v{version:apiVersion}/admin/...`.
Reason: Route discovery is less predictable when a single area changes root segment naming.

CONTROLLER CONSISTENCY ISSUE
Controller: `OperationManagerDashboardController`
Endpoint: `GET /api/v{version:apiVersion}/operation-manager/dashboard`
File: `Src/TransitNova.Api/Controllers/OperationManager/Query/OperationManager/OperationManagerDashboardController.cs`
Current Behavior: Uses the singular base route `operation-manager` while the rest of the operation-manager surface uses `operation-managers`.
Expected Behavior: Use the same plural base segment as the related controller groups.
Reason: Consumers have to switch naming conventions within the same resource area.

CONTROLLER CONSISTENCY ISSUE
Controller: `WarehouseManagerAdminController`, `WarehouseManagerCarriersController`, `WarehouseManagerShipmentsController`, `WarehouseManagerTripsController`, `WarehouseManagerController`
Endpoint: `multiple warehouse-manager routes`
File: `Src/TransitNova.Api/Controllers/Admin/WarehouseManager/WarehouseManagerAdminController.cs`, `Src/TransitNova.Api/Controllers/WarehouseManager/Carriers/WarehouseManagerCarriersController.cs`, `Src/TransitNova.Api/Controllers/WarehouseManager/Shipments/WarehouseManagerShipmentsController.cs`, `Src/TransitNova.Api/Controllers/WarehouseManager/Trips/WarehouseManagerTripsController.cs`, `Src/TransitNova.Api/Controllers/WarehouseManager/WarehouseManagerDashboard/WarehouseManagerController.cs`
Current Behavior: Three different route spellings are used for the same resource family: `warehousemanagers`, `warehouse-manager`, and `warehousemanager`.
Expected Behavior: Choose one canonical pluralized and hyphenated route convention across admin and user-facing surfaces.
Reason: Inconsistent hyphenation and pluralization makes the API harder to predict and document.

CONTROLLER CONSISTENCY ISSUE
Controller: `WarehouseManagerController`
Endpoint: `PUT /api/v{version:apiVersion}/warehousemanager/update`
File: `Src/TransitNova.Api/Controllers/WarehouseManager/WarehouseManagerDashboard/WarehouseManagerController.cs`
Current Behavior: This mutation endpoint has no `[IdempotencyKey]`, no granular permission policy, and advertises HTTP 200 even though the command returns `BaseResult.NoContent()`.
Expected Behavior: Add the same mutation safeguards used elsewhere in the API and align the documented response code with the command intent.
Reason: This is the only warehouse-manager mutation without the usual command protections, and its response metadata does not match the semantic intent of the command.

CONTROLLER CONSISTENCY ISSUE
Controller: `WarehouseManagerTripsController`
Endpoint: `GET /api/v{version:apiVersion}/warehousemanager/trips/{warehouseId: guid}`
File: `Src/TransitNova.Api/Controllers/WarehouseManager/Trips/WarehouseManagerTripsController.cs`
Current Behavior: The route constraint is written as `{warehouseId: guid}` with a space before `guid`.
Expected Behavior: Use the normal `{warehouseId:guid}` route template.
Reason: Malformed route templates are easy to misread and can behave differently than the rest of the API templates.

CONTROLLER CONSISTENCY ISSUE
Controller: `UserShipmentQueryController`, `UserShipmentOperationsController`, `UserShipmentsCreation`
Endpoint: `GET /api/v{version:apiVersion}/users/shipments/{shipmentId:guid}` versus `POST|PUT|PATCH|DELETE /api/v{version:apiVersion}/shipments/...`
File: `Src/TransitNova.Api/Controllers/User/UserShipmentOperations/UserShipmentQueryController.cs`, `Src/TransitNova.Api/Controllers/User/UserShipmentOperations/UserShipmentOperationsController.cs`, `Src/TransitNova.Api/Controllers/User/UserShipmentOperations/UserShipmentsCreation.cs`
Current Behavior: The same user-owned shipment resource is split across `/users/shipments` and `/shipments` base paths.
Expected Behavior: Keep query and mutation endpoints under the same resource root unless there is a clear bounded-context separation.
Reason: Mixed base routes make the user shipment API less predictable than the rest of the solution.

CONTROLLER CONSISTENCY ISSUE
Controller: `BundleController`
Endpoint: `POST /api/v{version:apiVersion}/admin/bundles`
File: `Src/TransitNova.Api/Controllers/Admin/Bundles/BundleController.cs`
Current Behavior: Declares `ProducesResponseType(StatusCodes.Status200OK)` but the handler returns `BaseResult.Created(...)`, which `ToActionResult()` maps to HTTP 201.
Expected Behavior: Document HTTP 201 Created as the success response.
Reason: Generated OpenAPI descriptions and client expectations will advertise the wrong success code.

## 6. Verification
- `dotnet build Src/TransitNova.InfraStructure/TransitNova.InfraStructure.csproj` succeeded after the `MemoryCacheService` update.

## 7. Scores
- Overall cache consistency score: `4/10`
- Overall API consistency score: `6/10`

## 8. Final Status
- Remaining cache issues still exist.
- Remaining controller inconsistencies still exist.