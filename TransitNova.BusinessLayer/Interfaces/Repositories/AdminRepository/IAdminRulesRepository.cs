namespace TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository
{
    public interface IAdminRulesRepository
    {
        Task<bool> IsAdminExistsAsync(Guid adminId, CancellationToken ct);
    }
}
