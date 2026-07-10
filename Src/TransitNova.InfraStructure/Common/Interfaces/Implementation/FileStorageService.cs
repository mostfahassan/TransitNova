using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using TransitNova.InfraStructure.Common.Interfaces.Contract;
namespace TransitNova.InfraStructure.Common.Interfaces.Implementation
{
    internal class FileStorageService(IWebHostEnvironment environment, ILogger<FileStorageService> logger) : IFileStorageService
    {
        private readonly string _rootPath = Path.Combine(environment.ContentRootPath, "ReportStorage");
        public Task<bool> DeleteFileAsync(string filePath, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(filePath)|| !File.Exists(filePath))
                    return Task.FromResult(true);

                File.Delete(filePath);

                return Task.FromResult(true);

            }


            catch (IOException ex)
            {
                logger.LogWarning(ex, "Could not delete file because it is in use. File: {FilePath}", filePath);
                return Task.FromResult(false);
            }
            catch (UnauthorizedAccessException ex)
            {
                logger.LogError(ex, "Permission denied when trying to delete file. File: {FilePath}", filePath);
                return Task.FromResult(false);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An unexpected error occurred while deleting file. File: {FilePath}", filePath);
                return Task.FromResult(false);
            }
        }
    
        public async Task<string> SaveFileAsync(byte[] content, string folder, string fileName, CancellationToken cancellationToken)
        {
            var directory = Path.Combine(_rootPath, folder);

            Directory.CreateDirectory(directory);

            var filePath = Path.Combine(directory, fileName);

            await File.WriteAllBytesAsync(filePath, content, cancellationToken);

            return filePath;
        }

    }
}
