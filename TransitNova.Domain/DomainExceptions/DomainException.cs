using System;

namespace TransitNova.Domain.DomainExceptions
{
    public abstract class DomainException : Exception
    {
        public string ErrorCode { get; }
        public string? EntityType { get; }
        public Guid? EntityId { get; }

        protected DomainException(string message, string errorCode, string? entityType = null, Guid? entityId = null) : base(message)
        {
            ErrorCode = errorCode;
            EntityType = entityType;
            EntityId = entityId;
        }
    }

    public sealed class InvalidShipmentStateException : DomainException
    {
        public InvalidShipmentStateException(Guid shipmentId, string expected, string actual)
            : base($"Shipment {shipmentId} is in {actual} state. Expected: {expected}", "INVALID_SHIPMENT_STATE", "Shipment", shipmentId) { }
    }
    public sealed class VehicleCapacityExceededException : DomainException
    {
        public VehicleCapacityExceededException(Guid vehicleId, decimal weight, decimal volume)
            : base($"Vehicle {vehicleId} capacity exceeded. Weight: {weight}, Volume: {volume}", "VEHICLE_CAPACITY_EXCEEDED", "Vehicle", vehicleId) { }
    }

    public class TripPlanningException : DomainException
    {
        public TripPlanningException(string message, Guid? tripId = null)
            : base(message, "TRIP_PLANNING_ERROR", "Trip", tripId) { }
    }

    
    public class RefreshTokenNotFoundException : DomainException
    {
        public RefreshTokenNotFoundException()
            : base("Refresh Token Not Found", "REFRESH_TOKEN_NOT_FOUND") { }
    }

    public class InvalidRefreshTokenException : DomainException
    {
        public InvalidRefreshTokenException()
            : base("Refresh Token Is Not Valid", "REFRESH_TOKEN_NOT_VALID") { }
    }
    public class InvalidCarrierStatusException : DomainException
    {
        public InvalidCarrierStatusException()
            : base("carrier not in valid Status.", "INVALID_CARRIER_STATUS") { }
    }
    public class ReusedRefreshTokenException : DomainException
    {
        public ReusedRefreshTokenException(Guid userId , string token)
            : base($"Refresh token reuse detected. UserId: {userId}, Token: {token}", "REUSED_REFRESH_TOKEN_EXCEPTION") { }
    }

    public class ShipmentNotAssignedException : DomainException
    {
        public ShipmentNotAssignedException(Guid shipmentId , Guid carrierId)
            : base($"Shipment With Id {shipmentId} Not Assigned For Carrier With Id {carrierId}", "SHIPMENT_NOT_ASSIGNED_EXCEPTION") { }
    }



}
