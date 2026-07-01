using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Bundles.Segregations.Query
{
    internal interface IAdminBundlesQuery : IGetBundleByIdQueryService, IGetBundlesQueryService
    {
    }
}
