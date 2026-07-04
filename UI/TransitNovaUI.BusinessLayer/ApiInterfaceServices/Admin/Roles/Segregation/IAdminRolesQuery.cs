using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Queries;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Segregation
{
    public interface IAdminRolesQuery : IGetRoleByIdQueryService, IGetRoleMembersQueryService, IGetRolesQueryService
    {
    }
}

