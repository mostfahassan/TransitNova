using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Reports.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Shared.Reports.Segregation;

public interface ISharedReportsCommand : IRequestBundleReportCommandService, IRequestDashboardReportCommandService
{
}