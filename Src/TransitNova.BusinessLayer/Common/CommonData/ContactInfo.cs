namespace TransitNova.BusinessLayer.Common.CommonData
{
    internal sealed class ContactInfo
    {
        public string Phone { get;  set; }
        public string Address { get; set; }
        public ContactInfo(string phone, string address)
        {
            Phone = phone;
            Address = address;
        }
    }
}