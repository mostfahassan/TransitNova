using TransitNova.Domain.Enums.Bundle;

namespace TransitNova.BusinessLayer.Common.CommonData
{
    public class CommonBundleData
    {
        public string BundleName { get; set; } = string.Empty;
        public string BundleDescription { get; set; } = string.Empty;
        public decimal BundlePrice { get; set; }
        public BundleTier Tier { get; set; }
        public int BundleDurationMonths { get; set; }
  
        public int MaxShipmentsPerMonth { get; set; }
        public decimal MaxWeightPerShipment { get; set; }
        public decimal MaxDistancePerShipment { get; set; }

        public decimal DiscountPercentage { get; set; }
        public decimal MinimumShipmentValueForDiscount { get; set; }
    }
}
