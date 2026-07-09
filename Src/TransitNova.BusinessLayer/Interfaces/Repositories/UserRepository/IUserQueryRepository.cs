using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.Shipment;
using TransitNova.BusinessLayer.DTOs.UserProfile;
using TransitNova.Domain.Enums.Shipment;
namespace TransitNova.BusinessLayer.Interfaces.Repositories.UserRepository
{
    public interface IUserQueryRepository
    {
        Task<Guid> GetAppUserIdAsync(Guid AppUserId, CancellationToken ct);
        Task <string?> GetUserFullName (Guid AppUserId, CancellationToken ct);
        Task<UserProfileDto?> GetUserProfileAsync(Guid UserId, CancellationToken ct);
        Task<IEnumerable<RetrieveShipmentSummaryDto>> GetUserShipmentsAsync(Guid AppUserId, CancellationToken ct);
        Task<RetrieveShipmentDto?> GetUserShipmentDetailsAsync(Guid AppUserId, Guid shipmentId, CancellationToken ct);
        Task<Dictionary<ShipmentStatuses, int>> GetShipmentCountInStatusAsync(Guid AppUserId, CancellationToken cancellationToken);
        Task<AdminUserDetailsDto?> GetUserDetailsForAdminAsync(Guid userId, CancellationToken ct);
        Task<PagedResult<AdminUserDetailsDto>> FilterUsersAsync(UserFiltrationDto filter, CancellationToken ct);
    }
}