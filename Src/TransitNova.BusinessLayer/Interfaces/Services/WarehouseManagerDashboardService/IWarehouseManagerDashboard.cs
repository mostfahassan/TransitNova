using TransitNova.BusinessLayer.DTOs.WarehouseManager;
namespace TransitNova.BusinessLayer.Interfaces.Services.WarehouseManagerDashboardService
{
    public interface IWarehouseManagerDashboard
    {
        Task<WarehouseManagerDashboardDto> BuildAsync(Guid managerId ,CancellationToken cancellationToken);
    }
}
