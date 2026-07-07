using TransitNova.Domain.Enums.Result;
namespace TransitNova.BusinessLayer.Common.ResultPattern;
public static class Errors
{
    public static Error Validation(string message) => new(ErrorCode.VALIDATION_ERROR, message);
    public static Error Unauthorized(string message) => new(ErrorCode.UNAUTHORIZED, message);
    public static Error Forbidden(string message) => new(ErrorCode.FORBIDDEN, message);
    public static Error NotFound(string message) => new(ErrorCode.NOT_FOUND, message);
    public static Error Conflict(string message) => new(ErrorCode.CONFLICT, message);
    public static Error FailedOperation(string message) => new(ErrorCode.FAILED, message);

    // ── Domain-Specific ─────────────────────────────

    public static Error ShipmentNotFound(string message) => new(ErrorCode.SHIPMENT_NOT_FOUND, message);
    public static Error UserNotFound(string message) => new(ErrorCode.USER_NOT_FOUND, message);
    public static Error CarrierNotFound(string message) => new(ErrorCode.CARRIER_NOT_FOUND, message);
    public static Error TripNotFound(string message) => new(ErrorCode.TRIP_NOT_FOUND, message);
    public static Error ZoneNotFound(string message) => new(ErrorCode.ZONE_NOT_FOUND, message);
    public static Error InvalidShipmentState(string message) => new(ErrorCode.INVALID_SHIPMENT_STATE, message);
    public static Error InvalidCarrierState(string message) => new(ErrorCode.INVALID_CARRIER_STATE, message);
    public static Error TrackingNumberAlreadyExists(string message) => new(ErrorCode.TRACKING_NUMBER_ALREADY_EXISTS, message);
    public static Error EmailAlreadyExists(string message) => new(ErrorCode.EMAIL_ALREADY_EXISTS, message);
    public static Error InvalidCredentials(string message) => new(ErrorCode.INVALID_CREDENTIALS, message);
    public static Error CarrierNotAvailable(string message) => new(ErrorCode.CARRIER_NOT_AVAILABLE, message);
    public static Error FailedToAssign(string message) => new(ErrorCode.FAILED_TO_ASSIGN, message);
    public static Error BundleNotFound(string message) => new(ErrorCode.BUNDLE_NOT_FOUND, message);
    public static Error ShipmentCreationFailed(string message) => new(ErrorCode.SHIPMENT_CREATION_FAILED, message);
    public static Error CarrierNotActive(Guid carrierId) => new(ErrorCode.CARRIER_NOT_ACTIVE, $"Carrier {carrierId} is not active.");
    public static Error ShipmentNotAssigned(Guid shipmentId, Guid carrierId) => new(ErrorCode.SHIPMENT_NOT_ASSIGNED, $"Shipment {shipmentId} is not assigned to Carrier {carrierId}.");
    public static Error InvalidCarrierStatus() => new(ErrorCode.INVALID_CARRIER_STATUS, "Carrier is not in valid status.");
    public static Error CarrierWithNoWarehouse() => new(ErrorCode.NO_CARRIER_WAREHOUSE_FOUND, "Carrier does not have a home warehouse.");
    public static Error RefreshTokenNotFound() => new(ErrorCode.REFRESH_TOKEN_NOT_FOUND, "Refresh token not found.");
    public static Error InvalidRefreshToken() => new(ErrorCode.REFRESH_TOKEN_NOT_VALID, "Refresh token is not valid.");
    public static Error VehicleNotFound(string message) => new(ErrorCode.VEHICLE_NOT_FOUND, message);
}
