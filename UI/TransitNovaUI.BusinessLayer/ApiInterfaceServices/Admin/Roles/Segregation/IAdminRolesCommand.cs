using System;
using System.Collections.Generic;
using System.Text;
using TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Commands;

namespace TransitNovaUI.BusinessLayer.ApiInterfaceServices.Admin.Roles.Segregation
{
    internal interface IAdminRolesCommand : ICreateRoleCommandService, IDeleteRoleCommandService, IUpdateRoleCommandService, IUpdateRoleMembersCommandService
    {
    }
}
