namespace TransitNova.BusinessLayer.Common.CommonData
{
    public class CommonBundleData
    {
       public string BundleName { get; set; } = string.Empty;
        public decimal TotalWeight { get; set; }
        public decimal BundlePrice { get; set; }
        public string BundleDescription { get; set; } = string.Empty;
        public decimal TotalDistance { get; set; }
        public int TotalShipments { get; set; }
    }
}
