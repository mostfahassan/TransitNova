namespace TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository
{
    public interface IAdminQueryRepository
    {
        Task<string> GetAdminNameAsync(Guid adminId, CancellationToken cancellationToken);
        Task<List<Guid>> GetAdminIdsAsync(CancellationToken cancellationToken);
    }
}
