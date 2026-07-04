using TransitNova.Domain.Contracts.Roles;

namespace TransitNova.UI.Infrastructure.Mvc;

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


