
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.Admin;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.Admin
{
    public class AdminRulesRepository(AppDbContext context) : IAdminRulesRepository, IAdminQueryRepository
    {
        public async Task<bool> IsAdminExistsAsync(Guid adminId, CancellationToken ct)
         => await context.Admins.AnyAsync(admin => admin.Id == adminId, ct);

        public async Task<string> GetAdminNameAsync(Guid adminId, CancellationToken ct)
            => await context.Admins
                .AsNoTracking()
                .Where(admin => admin.AppUserId == adminId)
                .Select(admin => admin.FullName)
                .FirstOrDefaultAsync(ct) ?? string.Empty;
    }
}
