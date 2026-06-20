using TransitNova.BusinessLayer.Common.CommonData;
namespace TransitNova.BusinessLayer.DTOs.CarrierCompany
{
    public class RetrieveCarrierCompany : BaseCarrierCompanyData
    {
        public string Code { get; set; } = string.Empty;
        public string CityName { get; set; } = string.Empty;
        public string CountryName { get; set; } = string.Empty;
        public DateTime ContractStartDate { get; set; }
        public DateTime ContractEndDate { get; set; }
    }
}
