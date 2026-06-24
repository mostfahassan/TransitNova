
using TransitNova.BusinessLayer.DTOs.Admin;
namespace TransitNova.BusinessLayer.Interfaces.Services.AdminDashboard
{
    public interface IAdminDashboard
    {
        Task<AdminDashboardDto> BuildAsync(CancellationToken cancellationToken);
    }
}
