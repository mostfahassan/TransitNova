namespace TransitNova.InfraStructure.Common.Interfaces.Contract
{
    public interface IFileStorageService
    {
       Task<string> SaveFileAsync(byte[] content, string folder, string fileName, CancellationToken cancellationToken);
        Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken);
    }
}
