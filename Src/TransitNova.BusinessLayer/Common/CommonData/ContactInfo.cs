using TransitNova.Domain.Entities.Common;

namespace TransitNova.BusinessLayer.Common.CommonData
{
    internal sealed class ContactInfo
    {
        public string Phone { get;  set; }
        public Address Address { get; set; }
        public ContactInfo(string phone, Address address)
        {
            Phone = phone;
            Address = address;
        }
    }
}