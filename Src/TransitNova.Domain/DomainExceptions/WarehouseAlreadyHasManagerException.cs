namespace TransitNova.Domain.DomainExceptions
{
    public class WarehouseAlreadyHasManagerException : DomainException
    {
        public WarehouseAlreadyHasManagerException(string message, Guid? warehouseId = null)
            : base(message, "WAREHOUSE_ALREADY_HAS_MANAGER", "Warehouse", warehouseId) { }
    }

}
