
namespace TransitNova.BusinessLayer.Interfaces.Repositories.Admin
{
    public interface IAdminRulesRepository
    {
        Task<bool> IsAdminExistsAsync(Guid adminId, CancellationToken ct);
    }
}
