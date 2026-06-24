namespace TransitNova.BusinessLayer.Common.CommonData
{
    public class CreateReceiverDto
    {
        public string FirstName { get; set; } = string.Empty;
        public Guid SenderId { get; set; }
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public int CityId { get; set;}
       
    }
}
