
using System.Runtime.CompilerServices;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
using TransitNova.Domain.Enums.Warehouse;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class Warehouse : BaseEntity<Guid>
    {

        private readonly List<Zone> _zonesServed = []; 
        private readonly List<Carrier> _carriers = [];
        public string Name { get; private set; } = string.Empty;
        public WarehouseType Type { get; private set; }
        public bool HasManager { get; private set; } = false;
        public string Address { get; private set; } = string.Empty;
        public byte[] RowVersion { get; private set; } = default!;
        public virtual ICollection<Trip> Trips { get; set; } = new List<Trip>();
        public decimal Capacity { get; private set; }
        public decimal CurrentUsage { get; private set; }
        public IReadOnlyCollection<Zone> ZonesServed => _zonesServed.AsReadOnly();
        public IReadOnlyCollection<Carrier> Carriers => _carriers.AsReadOnly();
        public WarehouseManagerProfile Manager { get; private set; } = null!;
        public Guid ManagerId { get; private set; }
        public int? OperatingHours { get; private set; }
        private Warehouse()
        {

        }

        private Warehouse(string name, WarehouseType type, decimal capacity, decimal currentUsage, int operatingHours, string address, Guid createdBy,Guid managerId)
        {
            Validate(name, capacity, currentUsage, operatingHours, address);
            EnsureNoManager();
            Id = Guid.CreateVersion7();
            Name = name.Trim();
            Type = type;
            Capacity = capacity;
            CurrentUsage = currentUsage;
            OperatingHours = operatingHours;
            CreatedBy = createdBy.ToString();
            Address = address;
            CurrentState = true;
            ManagerId = managerId;
            HasManager = true;
        }

       
        public static Warehouse Create(string name, WarehouseType type, decimal capacity, decimal currentUsage, int operatingHours, string address, Guid createdBy, Guid managerId)
            => new (name, type, capacity, currentUsage, operatingHours, address, createdBy, managerId);

        public void Update(Guid updatedBy, string? name = null, WarehouseType? type =null ,decimal? capacity = null, decimal? currentUsage = null, int? operatingHours = null, string? address = null, Guid? managerId = null)
        {
            Validate(name, capacity, currentUsage, operatingHours, address);
            EnsureNotSameManager(managerId);
            Name = name?.Trim() ?? Name;
            Type = type ?? Type;
            Capacity = capacity ?? Capacity;
            CurrentUsage = currentUsage ?? CurrentUsage;
            OperatingHours = operatingHours ?? OperatingHours;
            Address = address?.Trim() ?? Address;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy.ToString();
            ManagerId = managerId ?? ManagerId;
        }
     

        public void AddZone(Zone zone)
        {
            if (zone is null)
                throw new DomainArgumentException(nameof(zone), "Zone cannot be null.", "ZONE_REQUIRED", "Warehouse", Id);

            if (!_zonesServed.Any(z => z.Id == zone.Id))
            {
                _zonesServed.Add(zone);
            }
        }
        public void RemoveZone(Zone zone)
        {
            _zonesServed.Remove(zone);
        }

        public void ReplaceZones(IEnumerable<Zone> zones, Guid updatedBy)
        {
            if (zones is null)
                throw new DomainArgumentException(nameof(zones), "Zones cannot be null.", "ZONES_REQUIRED", "Warehouse", Id);

            var distinctZones = zones
                .GroupBy(z => z.Id)
                .Select(g => g.First())
                .ToList();

            _zonesServed.Clear();
            _zonesServed.AddRange(distinctZones);
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = updatedBy.ToString();
        }

        private void EnsureNoManager ()
        {
            if (HasManager)
                throw new WarehouseAlreadyHasManagerException("Warehoues Already Has Manager Can't Have Another Manager", Id);
        }
        private void EnsureNotSameManager (Guid? managerId)
        {
            if (managerId.HasValue)
            {
                if (managerId == ManagerId)
                    throw new SameWarehouseManagerException("Warehoues Already Has Same Manager Can't Assign Same Manager Twice", Id);
            }
        }

        // Validations
        private static void Validate(string? name =null, decimal? capacity = null , decimal? currentUsage = null, int? operatingHours = null, string? address = null)
        {
            if (name is not null && string.IsNullOrWhiteSpace(name) )
                throw new DomainArgumentException(nameof(name), "Warehouse name is required.", "WAREHOUSE_NAME_REQUIRED", "Warehouse");

            if (address is not null&& string.IsNullOrWhiteSpace(address))
                throw new DomainArgumentException(nameof(address), "Warehouse address is required.", "WAREHOUSE_ADDRESS_REQUIRED", "Warehouse");

            if (capacity is not null && capacity <= 0)
                throw new DomainArgumentOutOfRangeException(nameof(capacity), "Warehouse capacity must be greater than zero.", "WAREHOUSE_CAPACITY_INVALID", "Warehouse");

            if (currentUsage is not null && currentUsage < 0)
                throw new DomainArgumentOutOfRangeException(nameof(currentUsage), "Warehouse current usage cannot be negative.", "WAREHOUSE_USAGE_INVALID", "Warehouse");

            if (currentUsage > capacity)
                throw new DomainOperationException("Warehouse current usage cannot exceed capacity.", "WAREHOUSE_USAGE_EXCEEDS_CAPACITY", "Warehouse");

            if (operatingHours.HasValue && operatingHours.Value <= 0)
                throw new DomainArgumentOutOfRangeException(nameof(operatingHours), "Warehouse operating hours must be greater than zero.", "WAREHOUSE_OPERATING_HOURS_INVALID", "Warehouse");
        }

    }
}

