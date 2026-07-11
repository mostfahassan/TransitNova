using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using TransitNova.Domain.Contracts.DomainEvents.Events.CarrierEvents;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Users;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class Carrier : BaseInfo<Guid>, ISoftDeletable
    {
        [NotMapped]
        private int RemainingShipmentsCount => MaxDailyShipments - AssignedShipmentsCount;

        public string Code { get; private set; } = string.Empty;
        public string LicenseNumber { get; private set; } = string.Empty;
        public int EstimatedDeliveryDays { get; private set; }

        public int CompletedShipmentsCount { get; private set; }
        public int AssignedShipmentsCount { get; private set; } = default;
        public int MaxDailyShipments { get; private set; }

        public bool HasAdditionalInfo { get; private set; }

        public decimal DefaultCostPerKg { get; private set; }
        public int YearsOfExperience { get; private set; }

        public decimal SuccessRate { get; private set; } = default;
        public decimal AverageRating { get; private set; }
        public int TotalRatings { get; private set; }
        public bool IsDeleted { get; private set; }
        public DateTime? DeletedOn { get; private set; }
        public CarrierStatus Status { get; private set; }

        public DateTime ContractStartDate { get; private set; }
        public DateTime ContractEndDate { get; private set; }

        public Guid? HandlerId { get; private set; }
        public DateTime HandledAt { get; private set; }

        public virtual OperationManagerProfile? HandledBy { get; private set; }
        public virtual ICollection<Trip> Trips { get; } = new List<Trip>();
        public virtual ICollection<Zone> ServedZones { get;} = new List<Zone>();

        public virtual Vehicle Vehicle { get;} = null!;

        public Guid? HomeWarehouseId { get; private set; }
        public Warehouse? HomeWarehouse { get; set; }

        public Guid AppUserId { get; private set; }

        public byte[] RowVersion { get; private set; } = default!;

        public Guid TripId { get; private set; }
        private Carrier() { }

        private Carrier(Guid appUserId, string firstName, string lastName, string email, string phone, Address address, int cityId)
        {
            Id = Guid.CreateVersion7();
            AppUserId = appUserId;
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phone;
            Address = address;
            CityId = cityId;
            CreatedBy = appUserId.ToString();
            UserType = UserType.Carrier;
            Status = CarrierStatus.Available;
            ContractStartDate = DateTime.UtcNow;
            CompletedShipmentsCount = 0;
            AssignedShipmentsCount = 0;
            GenerateUniqueCode();
            CurrentState = true;
        }

        public static Carrier Create(Guid appUserId, string firstName, string lastName, string email, string phone, Address address, int cityId)
        {
            var carrier = new Carrier(appUserId, firstName, lastName, email, phone, address, cityId);

            carrier.RaiseDomainEvent(new CarrierCreatedDomainEvent(carrier.Id,carrier.FullName, phone, carrier.Code));

            return carrier;
        }

        private void GenerateUniqueCode()
        {
            const string prefix = "CR";
            var datePart = DateTime.UtcNow.ToString("yyMMdd");

            var randomPart = Convert.ToBase64String(RandomNumberGenerator.GetBytes(6))
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_')
                .ToUpperInvariant();

            Code = $"{prefix}-{datePart}-{randomPart}";
        }

        private void StartContract(DateTime startDate, int contractYears)
        {
            ContractStartDate = startDate;
            ContractEndDate = startDate.AddYears(contractYears);
        }

        private void Assign(Guid operationManagerId)
        {
            EnsureAvailability();
            EnsureDailyShipmentLimit();
            AssignedShipmentsCount++;
            HandlerId = operationManagerId;
            HandledAt = DateTime.UtcNow;
        }

        private void UpdateCarrierStatus(CarrierStatus newStatus, Guid? handlerId = null)
        {
            if (newStatus == Status) return;

            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;

            UpdatedBy = handlerId.HasValue
                ? handlerId.Value.ToString()
                : Id.ToString();
        }

        public void AddAdditionalData(
            Guid userId,
            string license,
            int maxDailyShipment,
            decimal defaultCost,
            int experience,
            DateTime startDate,
            int contractYears,
            Guid? homeWarehouseId)
        {
            EnsureHasAdditionalInfo();

            LicenseNumber = license;
            MaxDailyShipments = maxDailyShipment;
            DefaultCostPerKg = defaultCost;
            YearsOfExperience = experience;
            HomeWarehouseId = homeWarehouseId;
            StartContract(startDate, contractYears);
            HasAdditionalInfo = true;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = userId.ToString();
            RaiseDomainEvent(new CarrierAdditionalInfoAddedDomainEvent(Id, LicenseNumber, MaxDailyShipments));
        }

        public void UpdateProfile(Guid userId, string? firstName, string? lastName, string? phoneNumber, Address? address)
        {
            FirstName = firstName?.Trim() ?? FirstName;
            LastName = lastName?.Trim() ?? LastName;
            PhoneNumber = phoneNumber?.Trim() ?? PhoneNumber;
            Address = address ?? Address;

            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = userId.ToString();

            RaiseDomainEvent(new CarrierProfileUpdatedDomainEvent(Id));
        }

        public void AssignToPickup(Guid operationManagerId)
        {
            Assign(operationManagerId);
            UpdateCarrierStatus(CarrierStatus.AssignedToPickUpShipment, operationManagerId);
            RaiseDomainEvent(new CarrierAssignedToPickupDomainEvent(Id, AssignedShipmentsCount, CarrierStatus.AssignedToPickUpShipment));
        }
        public void AssignToDeliver(Guid operationManagerId)
        {
            Assign(operationManagerId);
            UpdateCarrierStatus(CarrierStatus.AssignedToDeliveryShipment, operationManagerId);
            RaiseDomainEvent(new CarrierAssignedToDeliveryDomainEvent(Id, AssignedShipmentsCount, CarrierStatus.AssignedToDeliveryShipment));
        }

        public void AssignToTrip(Guid operationManagerId, Guid tripId)
        {
            EnsureStatus();
            EnsureItsCarrierTrip(tripId);
            TripId = tripId;
            HandlerId = operationManagerId;
            HandledAt = DateTime.UtcNow;
           
            UpdateCarrierStatus(CarrierStatus.OnTrip, operationManagerId);
            RaiseDomainEvent(new CarrierTripStartedDomainEvent(Id, CarrierStatus.OnTrip));
        }

        public void CompleteShipment()
        {
            EnsureCompleteShipmentCount();
            AssignedShipmentsCount--;
            CompletedShipmentsCount++;

            UpdateCarrierStatus(
                AssignedShipmentsCount > 0
                    ? CarrierStatus.OnTrip
                    : CarrierStatus.Available);

            RaiseDomainEvent(new CarrierShipmentCompletedDomainEvent(Id, AssignedShipmentsCount, RemainingShipmentsCount, CompletedShipmentsCount));

        }

        public void AddRating(int rating)
        {
            EnsureRating(rating);

            AverageRating = ((AverageRating * TotalRatings) + rating) / (TotalRatings + 1);
            TotalRatings++;
            RaiseDomainEvent(new CarrierRatedDomainEvent(Id, rating, AverageRating, TotalRatings));
        }



        //Validations 

        private void EnsureAvailability()
        {
            if (Status != CarrierStatus.Available)
                throw new InvalidCarrierStatusException();
        }

        private void EnsureDailyShipmentLimit()
        {
            if (RemainingShipmentsCount <= 0)
                throw new DomainOperationException("Carrier reached daily limit.", "CARRIER_DAILY_LIMIT_REACHED", "Carrier", Id);
        }
        private void EnsureCompleteShipmentCount()
        {
            if (AssignedShipmentsCount <= 0)
                throw new DomainOperationException("No assigned shipments.", "NO_ASSIGNED_SHIPMENTS", "Carrier", Id);
        }

        private void EnsureHasAdditionalInfo()
        {
            if (HasAdditionalInfo)
                throw new DomainOperationException("Additional info already exists.", "CARRIER_ADDITIONAL_INFO_EXISTS", "Carrier", Id);

        }

        private void EnsureRating(int rating)
        {
            if (rating < 1 || rating > 5)
                throw new DomainOperationException($"Rating is {rating}. Must be between 1 and 5.", "CARRIER_RATING_INVALID", "Carrier", Id);
        }
        private void EnsureStatus()
        {
            if (Status is not CarrierStatus.AssignedToPickUpShipment and not CarrierStatus.AssignedToDeliveryShipment)
                throw new DomainOperationException("Carrier must have assigned shipments before starting a trip.", "INVALID_CARRIER_STATE", "Carrier", Id);
            if (Status is CarrierStatus.OnTrip)
                throw new DomainOperationException("Carrier Is Already In A Trip Can't Be Assigned To Another.", "CARRIER_ALREADY_IN_TRIP", "Carrier", Id);
        }
        private void EnsureItsCarrierTrip(Guid tripId)
        {
            if (Trips is null ||Trips.Count == 0)
                throw new DomainOperationException(
                    "Trips are not loaded for validation.",
                    "TRIPS_NOT_LOADED",
                    nameof(Carrier),
                    Id);

            if (!Trips.Any(t => t.Id == tripId))
                throw new DomainOperationException(
                    "Carrier is not assigned to this trip",
                    "INVALID_TRIP_ASSIGNING_OPERATION",
                    nameof(Carrier),
                    tripId);
        }

    }
}