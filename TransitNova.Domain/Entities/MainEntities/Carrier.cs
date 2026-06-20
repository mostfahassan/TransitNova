
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Cryptography;
using TransitNova.BusinessLayer.Interfaces;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Carrier;
using TransitNova.Domain.Enums.Users;

namespace TransitNova.Domain.Entities.MainEntities
{
    public class Carrier : BaseInfo , ISoftDeletable
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
        public DateTime ContractStartDate { get; private set;}
        public DateTime ContractEndDate { get; private set; }
        public Guid? HandlerId { get; private set;}
        public DateTime HandledAt { get; private set;}
        public virtual OperationManagerProfile? HandledBy { get; private set; }
        public Guid ? CompanyId { get; private set; }
        public virtual CarrierCompany? Company { get; set; }
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public virtual ICollection<Zone> ServedZones { get; set; } = new List<Zone>();
        public virtual Vehicle Vehicle { get; private set; } = null!;
        public Guid? HomeWarehouseId { get; private set; }
        public Warehouse? HomeWarehouse { get; private set; }
        public Guid AppUserId { get; private set; }
        public byte[] RowVersion { get; private set; } = default!;


        // Factory method for creating a new Carrier
        private Carrier()
        {

        }
        private Carrier(Guid Id, string firstName, string lastName, string email, string phone, string address, int cityId)
        {
            this.Id = Guid.CreateVersion7(); 
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            PhoneNumber = phone;
            Address = address;
            AppUserId = Id;
            CreatedAt = DateTime.UtcNow;
            CreatedBy = Id.ToString();
            UserType = UserType.Carrier;
            Status = CarrierStatus.Available;
            GenerateUniqueCode();
            ContractStartDate = DateTime.UtcNow;
            CompletedShipmentsCount = 0;
            CurrentState = true;
            CityId = cityId;
        }
      

        //Private Methods
        private void GenerateUniqueCode()
        {
            const string PREFIX = "CR";
            string datePart = DateTime.UtcNow.ToString("yyMMdd");
            string randomPart = Convert.ToBase64String(RandomNumberGenerator.GetBytes(6))
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_')
                .ToUpperInvariant();
            Code = $"{PREFIX}-{datePart}-{randomPart}";

        }
        private void StartContract(DateTime startDate, int ContractYears)
        {
            ContractStartDate = startDate;
            ContractEndDate = startDate.AddYears(ContractYears);
        }

        private void Assign(Guid OperationManagerId)
        {
            if (Status != CarrierStatus.Available)
                throw new InvalidCarrierStatusException();

            if (RemainingShipmentsCount <= 0)
                throw new DomainOperationException("Carrier has reached the maximum daily shipment limit.", "CARRIER_DAILY_LIMIT_REACHED", "Carrier", Id);
            AssignedShipmentsCount++;
            HandlerId = OperationManagerId;
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


        // Public Method 
        public static Carrier Create(Guid Id, string firstName, string lastName, string email, string phone, string address, int cityId)
        {
            return new (Id, firstName, lastName, email, phone, address, cityId);
        }
        public void AddAdditionalData(Guid userId, string license, int maxDailyShipment, Guid company, decimal defaultCost, int experience, DateTime startDate, int ContractYears, Guid? homeWarehouseId)
        {
            if (HasAdditionalInfo)
                throw new DuplicateShipmentInTripException("Additional info already added", "CARRIER_ADDITIONAL_INFO_EXISTS", "Carrier", Id);
            LicenseNumber = license;
            MaxDailyShipments = maxDailyShipment;
            CompanyId = company;
            DefaultCostPerKg = defaultCost;
            YearsOfExperience = experience;
            StartContract(startDate, ContractYears);
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = userId.ToString();
            HomeWarehouseId = homeWarehouseId;
            HasAdditionalInfo = true;
        }
        public void UpdateProfile(Guid userId, string? firstName, string? lastName, string? phoneNumber, string? address)
        {
            FirstName = firstName?.Trim() ?? FirstName;
            LastName = lastName?.Trim() ?? LastName;
            PhoneNumber = phoneNumber?.Trim() ?? PhoneNumber;
            Address = address?.Trim() ?? Address;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = userId.ToString();
        }
        public void AssignToPickup(Guid OperationManagerId)
        {
            Assign(OperationManagerId);
            UpdateCarrierStatus(CarrierStatus.AssignedToPickUpShipment, handlerId: OperationManagerId);
        }
        public void AssignToTrip(Guid OperationManagerId)
        {
            if (Status is not CarrierStatus.AssignedToPickUpShipment and not CarrierStatus.AssignedToDeliveryShipment)
                throw new DomainOperationException("Carrier must have assigned shipments before starting a trip.", "INVALID_CARRIER_TRIP_STATE", "Carrier", Id);

            HandlerId = OperationManagerId;
            HandledAt = DateTime.UtcNow;
            UpdateCarrierStatus(CarrierStatus.OnTrip, handlerId: OperationManagerId);

        }
        public void AssignToDeliver(Guid OperationManagerId)
        {
            Assign(OperationManagerId);
            UpdateCarrierStatus(CarrierStatus.AssignedToDeliveryShipment, handlerId: OperationManagerId);
        }
        public void CompleteShipment()
        {
            if (AssignedShipmentsCount <= 0)
                throw new DomainOperationException("No assigned shipments to complete.", "NO_ASSIGNED_SHIPMENTS", "Carrier", Id);

            AssignedShipmentsCount--;
            CompletedShipmentsCount++;
            UpdateCarrierStatus(
                   AssignedShipmentsCount > 0
                       ? CarrierStatus.OnTrip
                       : CarrierStatus.Available);
        }

        public void AddRating(int rating)
        {
            AverageRating = ((AverageRating * TotalRatings) + rating) / (TotalRatings + 1);
            TotalRatings++;
        }
    }
}
