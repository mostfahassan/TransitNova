
using TransitNova.BusinessLayer.Common.CommonData;

namespace TransitNova.BusinessLayer.DTOs.UserProfile
{
    public class UpdateUserProfile:CommonUpdateData 
    {
        public Guid? UserBundleId { get; set; }

    }
    public class UpdateWarehouseManagerProfile:CommonUpdateData 
    {
        public Guid? WarehouseId { get; set; }

    }
}
