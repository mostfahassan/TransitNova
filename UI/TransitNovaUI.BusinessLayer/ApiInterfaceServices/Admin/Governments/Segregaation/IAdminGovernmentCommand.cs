using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Governments.Segregaation
{
    public interface IAdminGovernmentCommand : ICreateGovernmentCommandService, IDeleteGovernmentCommandService, IUpdateGovernmentCommandService
    {
    }
}

