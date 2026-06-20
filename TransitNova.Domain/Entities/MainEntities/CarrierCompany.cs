using System.Security.Cryptography;
using TransitNova.Domain.DomainExceptions;
using TransitNova.Domain.Entities.Common;
namespace TransitNova.Domain.Entities.MainEntities
{
    public class CarrierCompany : BaseEntity<Guid>
    {
        private readonly List<Carrier> _carrierList = new ();
        public string Name { get; private set; } = string.Empty;
        public string Email { get; private set; } = string.Empty;
        public string Phone { get; private set; } = string.Empty;
        public string Code { get; private set; } = string.Empty;
        public string Address { get; private set; } = string.Empty;
        public DateTime ContractStartDate { get; private set; }
        public DateTime ContractEndDate { get; private set; }
        public Guid? ZoneId { get; private set; }
        public Zone? Zone { get; set; }
        public virtual IReadOnlyCollection<Carrier> Carriers => _carrierList;

        private void GenerateUniqueCode()
        {
            const string PREFIX = "COM";
            string datePart = DateTime.UtcNow.ToString("yyMMdd");
            string randomPart = Convert.ToBase64String(RandomNumberGenerator.GetBytes(9))
                .TrimEnd('=')
                .Replace('+', '-')
                .Replace('/', '_')
                .ToUpperInvariant();
            Code = $"{PREFIX}-{datePart}-{randomPart}";
        }
        private CarrierCompany()
        {
            
        }

        private CarrierCompany(string creatorId, string name, string email, string phone, string address, Guid? zoneId)
        {
            Id = Guid.CreateVersion7();
            Name = name;
            Email = email;
            Phone = phone;
            Address = address;
            ZoneId = zoneId;
            GenerateUniqueCode();
            CreatedAt = DateTime.UtcNow;
            CreatedBy = creatorId;
            CurrentState = true;
        }
        public static CarrierCompany Create(string creatorId, string name, string email, string phone, string address, Guid? zoneId)
        {
            return new (creatorId, name, email, phone, address, zoneId);
        }

        public void Update(string? name, string? email, string? phone, string? address, Guid? zoneId, string? userId)
        {
            Name = name ?? Name;
            Email = email ?? Email;
            Phone = phone ?? Phone;
            Address = address ?? Address;
            ZoneId = zoneId;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = userId;
        }

        public void UpdateContract(DateTime startDate, DateTime endDate, string userId)
        {
            ContractStartDate = startDate;
            ContractEndDate = endDate;
            UpdatedAt = DateTime.UtcNow;
            UpdatedBy = userId;
        }

        public void AddCarrier(Carrier carrier)
        {
            ArgumentNullException.ThrowIfNull(carrier);

            if (!_carrierList.Any(c => c.Id == carrier.Id))
            {
                _carrierList.Add(carrier);
            }
            else
            {
                throw new DuplicateShipmentInTripException("Carrier already exists in the company.", "CARRIER_ALREADY_IN_COMPANY", "CarrierCompany");
            }
        }
        public void RemoveCarrier(Carrier carrier)
        {
            if (carrier == null) return;

            if (!_carrierList.Any(c => c.Id == carrier.Id))
                throw new EntityNotFoundException("Carrier not found in the company.", "CARRIER_NOT_IN_COMPANY", "CarrierCompany");
            else
                _carrierList.Remove(carrier);
        }
    }
}