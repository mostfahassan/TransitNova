using TransitNova.BusinessLayer.Common.ResultPattern;
using TransitNova.BusinessLayer.DTOs.OperationManager;
namespace TransitNova.BusinessLayer.Interfaces.Services.OperationManagerDashboard
{
    public interface IOperationManagerDashboard
    {
        Task<Result<OperationManagerDashboardDto>> BuildAsync(Guid operationManagerId, CancellationToken cancellationToken);
    }
}
