namespace TransitNova.Domain.DomainExceptions
{
    public class SameWarehouseManagerException : DomainException
    {
        public SameWarehouseManagerException(string message, Guid? warehouseId = null)
            : base(message, "WAREHOUSE_ALREADY_HAS_SAME_MANAGER", "Warehouse", warehouseId) { }
    }

}
