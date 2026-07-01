using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Users.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Users.Segregation
{
    internal interface IAdminUserQuery : IFilterUsersQueryService, IGetUserDetailsQueryService
    {
    }
}
