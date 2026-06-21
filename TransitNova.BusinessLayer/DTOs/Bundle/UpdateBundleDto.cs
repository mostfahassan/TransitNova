
using TransitNova.BusinessLayer.Common.CommonData;
namespace TransitNova.BusinessLayer.DTOs.Bundle
{
    public class UpdateBundleDto : CommonBundleData
    {
        public Guid BundleId { get; set; }
    }
}
