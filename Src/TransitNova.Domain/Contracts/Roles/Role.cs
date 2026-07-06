using TransitNova.Domain.Enums.Users;
namespace TransitNova.Domain.Contracts.Roles;
public static class Role
{
    public const string Admin = nameof(UserType.Admin);
    public const string Carrier = nameof(UserType.Carrier);
    public const string OperationManager = nameof(UserType.OperationManager);
    public const string WarehouseManager = nameof(UserType.WarehouseManager);
    public const string User = nameof(UserType.User);
    public const string AllUsers = Admin + "," + Carrier + "," + OperationManager + "," + WarehouseManager + "," + User;
    public const string OperationManagerOrCarrier = Carrier + "," + OperationManager;
    public const string OperationManagerOrAdmin = OperationManager + "," + Admin;
    public const string OperationManagerCarrierOrUser = OperationManager + "," + Carrier + "," + User;
    public const string OperationManagerOrDriverOrAdmin = OperationManager + "," + Carrier + "," + Admin;
}