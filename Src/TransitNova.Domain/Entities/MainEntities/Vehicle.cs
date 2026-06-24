using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Carrier;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class Vehicle : BaseEntity<Guid>
    {
        public VehicleType VehicleType { get; private set; }
        public string PlateNumber { get; private set; } = string.Empty;
        public decimal CapacityWeight { get; private set; }
        public decimal CapacityVolume { get; private set; }
        public byte[] RowVersion { get; private set; } = default!;
        public bool IsRefrigerated { get; private set; }
        public bool IsActive { get; private set; } = true;
        public Guid CarrierId { get; private set; }
        public virtual Carrier Carrier { get;} = null!;

        private Vehicle()
        {
            
        }

        // Factory method for creating a new Vehicle
        private Vehicle(VehicleType vehicleType, string plateNumber, decimal capacityWeight, decimal capacityVolume, bool isRefrigerated, Guid carrierId)
        {
            Id = Guid.CreateVersion7();
            SetVehicleType(vehicleType);
            SetPlateNumber(plateNumber);
            SetCapacityWeight(capacityWeight);
            SetCapacityVolume(capacityVolume);
            SetRefrigerated(isRefrigerated);
            CarrierId = carrierId;
            IsActive = true;
        }
        public static Vehicle Create(VehicleType vehicleType, string plateNumber, decimal capacityWeight, decimal capacityVolume, bool isRefrigerated, Guid carrierId)
                      => new (vehicleType, plateNumber, capacityWeight, capacityVolume, isRefrigerated, carrierId);




        public void UpdateVehicle(
            VehicleType vehicleType,
            string plateNumber,
            decimal capacityWeight,
            decimal capacityVolume,
            bool isRefrigerated)
        {
            SetVehicleType(vehicleType);
            SetPlateNumber(plateNumber);
            SetCapacityWeight(capacityWeight);
            SetCapacityVolume(capacityVolume);
            SetRefrigerated(isRefrigerated);
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void ChangeCarrier(Guid carrierId)
        {
            if (carrierId == Guid.Empty)
                throw new DomainArgumentException(nameof(carrierId), "CarrierId cannot be empty.", "ARG_CARRIERID_EMPTY", "Vehicle");

            CarrierId = carrierId;
            UpdatedAt = DateTime.UtcNow;
        }

        private void SetVehicleType(VehicleType vehicleType)
        {
            if (!Enum.IsDefined<VehicleType>(vehicleType))
                throw new DomainArgumentOutOfRangeException(nameof(vehicleType), "Invalid vehicle type.", "ARG_VEHICLETYPE_OUT_OF_RANGE", "Vehicle");

            VehicleType = vehicleType;
        }

        private void SetPlateNumber(string plateNumber)
        {
            if (string.IsNullOrWhiteSpace(plateNumber))
                throw new DomainArgumentException(nameof(plateNumber), "PlateNumber is required.", "ARG_PLATENUMBER_REQUIRED", "Vehicle");

            if (plateNumber.Length > 50)
                throw new DomainArgumentException(nameof(plateNumber), "PlateNumber cannot exceed 50 characters.", "ARG_PLATENUMBER_LENGTH", "Vehicle");

            PlateNumber = plateNumber.Trim();
        }

        private void SetCapacityWeight(decimal capacityWeight)
        {
            if (capacityWeight <= 0)
                throw new DomainArgumentOutOfRangeException(nameof(capacityWeight), "CapacityWeight must be greater than zero.", "ARG_CAPACITY_WEIGHT_OUT_OF_RANGE", "Vehicle");

            CapacityWeight = capacityWeight;
        }

        private void SetCapacityVolume(decimal capacityVolume)
        {
            if (capacityVolume <= 0)
                throw new DomainArgumentOutOfRangeException(nameof(capacityVolume), "CapacityVolume must be greater than zero.", "ARG_CAPACITY_VOLUME_OUT_OF_RANGE", "Vehicle");

            CapacityVolume = capacityVolume;
        }

        private void SetRefrigerated(bool isRefrigerated)
        {
            IsRefrigerated = isRefrigerated;
        }
    }

 
}
