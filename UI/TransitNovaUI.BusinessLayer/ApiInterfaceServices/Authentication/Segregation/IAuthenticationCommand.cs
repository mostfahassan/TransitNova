using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Authentication.Segregation
{
    public interface IAuthenticationCommand :
        IChangePasswordCommandService, ILoginCommandService, IRefreshTokenCommandService, IRegisterCommandService, IRevokeRefreshTokenCommandService, ISignOutCommandService
    {
    }
}

