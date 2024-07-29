using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WeatherETL.Services
{
    public interface IBlobStorageService
    {
        Task UploadToBlobAsync(string filePath);
    }
    public class BlobStorageService : IBlobStorageService
    {
        private readonly string? _connectionString;
        private readonly string? _containerName;
        private readonly ILogger<BlobStorageService> _logger;
        private bool _enabled;

        public BlobStorageService(IConfiguration configuration, ILogger<BlobStorageService> logger) 
        {
            _connectionString = configuration.GetConnectionString("AzureStorageConnection");
            _containerName = configuration.GetValue<string>("BlobContainerName");
            _enabled = configuration.GetValue<bool>("AzureEnabled", true);
            _logger = logger;
        }

        public async Task UploadToBlobAsync(string filePath)
        {
            if (!_enabled) 
            {
                _logger.LogInformation($"AzureStorage is disabled, no need to upload to azure.");
                return;
            }
            _logger.LogInformation($"Uploading {filePath} to Azure Blob Storage...");
            var blobServiceClient = new BlobServiceClient(_connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync();

            var blobClient = containerClient.GetBlobClient(Path.GetFileName(filePath));
            await blobClient.UploadAsync(filePath, true);
            _logger.LogInformation("Upload completed successfully.");
        }
    }
}
