
using TransitNova.BusinessLayer.Common.CommonData;

namespace TransitNova.BusinessLayer.DTOs.UserProfile.OperationManager
{
    public class UpdateOperationManagerProfileDto: CommonUpdateData
    {
        public List<string> Permissions { get; set; } = new();
    }
}
