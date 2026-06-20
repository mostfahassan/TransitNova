
namespace TransitNova.BusinessLayer.Interfaces.Repositories.Admin
{
    public interface IAdminQueryRepository
    {
        Task<string> GetAdminNameAsync(Guid adminId, CancellationToken cancellationToken);
    }
}
