
namespace TransitNova.BusinessLayer.Common.CommonData
{
    public class BaseCarrierCompanyData  
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public Guid? ZoneId { get; set; }
    }
}
