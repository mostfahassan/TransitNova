namespace TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository
{
    public interface IAdminRulesRepository
    {
        Task<bool> AdminExistsAsync(Guid adminId, CancellationToken ct);
    }
}
