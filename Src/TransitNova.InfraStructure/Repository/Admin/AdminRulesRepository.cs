
using Microsoft.EntityFrameworkCore;
using TransitNova.BusinessLayer.Interfaces.Repositories.AdminRepository;
using TransitNova.InfraStructure.Context;
namespace TransitNova.InfraStructure.Repository.Admin
{
    public class AdminRulesRepository(AppDbContext context) : IAdminRulesRepository, IAdminQueryRepository
    {
        public async Task<bool> AdminExistsAsync(Guid adminId, CancellationToken ct)
         => await context.Admins.AnyAsync(admin => admin.Id == adminId, ct);

        public async Task<string> GetAdminNameAsync(Guid adminId, CancellationToken ct)
            => await context.Admins
                .AsNoTracking()
                .Where(admin => admin.AppUserId == adminId)
                .Select(admin => admin.FullName)
                .FirstOrDefaultAsync(ct) ?? string.Empty;

        public async Task<List<Guid>> GetAdminIdsAsync(CancellationToken cancellationToken)
          => await context.Admins.AsNoTracking()
            .Select(admin => admin.Id)
            .ToListAsync(cancellationToken);
    }
}
