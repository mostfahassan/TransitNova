
using TransitNova.BusinessLayer.Common.CommonData;
namespace TransitNova.BusinessLayer.DTOs.Bundle
{
    public class RetrieveBundleDto : CommonBundleData
    {
        public Guid Id { get; set; }
        public string TierName => Tier.ToString();
    }
}
