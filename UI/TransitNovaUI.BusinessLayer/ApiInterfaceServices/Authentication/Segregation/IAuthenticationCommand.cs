using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Segregation
{
    internal interface IAuthenticationCommand :
        IChangePasswordCommandService, ILoginCommandService, IRefreshTokenCommandService, IRegisterCommandService, IRevokeRefreshTokenCommandService, ISignOutCommandService
    {
    }
}
