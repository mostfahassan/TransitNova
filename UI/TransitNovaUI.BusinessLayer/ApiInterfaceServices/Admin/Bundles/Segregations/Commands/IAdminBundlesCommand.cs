using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Segregations.Commands
{
    public interface IAdminBundlesCommand : ICreateBundleCommandService, IUpdateBundleCommandService, IDeleteBundleCommandService
    {
    }
}
