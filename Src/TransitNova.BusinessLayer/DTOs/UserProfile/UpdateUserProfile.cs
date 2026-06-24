
using TransitNova.BusinessLayer.Common.CommonData;

namespace TransitNova.BusinessLayer.DTOs.UserProfile
{
    public class UpdateUserProfile:CommonUpdateData 
    {
        public int? UserBundleId { get; set; }

    }
}
