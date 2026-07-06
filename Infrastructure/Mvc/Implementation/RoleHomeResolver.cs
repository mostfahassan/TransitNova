using TransitNova.Domain.Contracts.Roles;
using TransitNova.UI.Infrastructure.Mvc.Interface;

namespace TransitNova.UI.Infrastructure.Mvc.Implementation;

public sealed class RoleHomeResolver : IRoleHomeResolver
{
    public string Resolve(string? role) => role switch
    {
        Role.Admin => "/AdminArea/Dashboard/Index",
        Role.User => "/UserArea/Dashboard/Index",
        Role.OperationManager => "/OperationManagerArea/Dashboard/Index",
        Role.WarehouseManager => "/WarehouseManagerArea/Dashboard/Index",
        Role.Carrier => "/CarrierArea/Dashboard/Index",
        _ => "/LandingArea/Home/Index"
    };
}


