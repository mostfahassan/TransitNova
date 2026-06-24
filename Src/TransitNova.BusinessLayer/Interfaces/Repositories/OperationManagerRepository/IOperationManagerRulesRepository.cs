namespace TransitNova.BusinessLayer.Interfaces.Repositories.OperationManagerRepository
{
    public interface IOperationManagerRulesRepository
    {
        Task<bool> ExistsAsync(Guid id, CancellationToken cancellationToken);
    }

}
