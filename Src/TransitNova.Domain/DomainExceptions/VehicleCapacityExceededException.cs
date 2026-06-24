using System;
namespace TransitNova.Domain.DomainExceptions
{
    public sealed class VehicleCapacityExceededException : DomainException
    {
        public VehicleCapacityExceededException(Guid vehicleId, decimal weight, decimal volume)
            : base($"Vehicle {vehicleId} capacity exceeded. Weight: {weight}, Volume: {volume}", "VEHICLE_CAPACITY_EXCEEDED", "Vehicle", vehicleId) { }
    }

}
