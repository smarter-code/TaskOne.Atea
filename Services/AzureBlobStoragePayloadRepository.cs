using Azure.Storage.Blobs;
using Microsoft.Extensions.Options;
using Services.Extensions;
using Services.Interfaces;
using Shared.Config;
using System.Text;

namespace Services
{
    public class AzureBlobStoragePayloadRepository : IPayloadRepository
    {
        private readonly BlobContainerClient _containerClient;

        public AzureBlobStoragePayloadRepository(IOptions<AzureBlobStorageConfig> azureBlobStorageConfigOptions)
        {
            var azureBlobStorageConfig = azureBlobStorageConfigOptions.Value;
            var blobServiceClient = new BlobServiceClient(azureBlobStorageConfig.ConnectionString);
            _containerClient = blobServiceClient.GetBlobContainerClient(azureBlobStorageConfig.LogPayloadStorageContainerName);

        }
        public async Task SavePayload(DateTime dateTime, string result)
        {
            await _containerClient.CreateIfNotExistsAsync();
            var blobClient = _containerClient.GetBlobClient($"{dateTime.GetPartitionKeyFormat()}\\{dateTime.GetRowKeyFormat()}");
            using MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(result));
            await blobClient.UploadAsync(memoryStream, overwrite: true);
        }

        public async Task<object> GetPayload(DateTime dateTime)
        {
            var blobClient = _containerClient.GetBlobClient($"{dateTime.GetPartitionKeyFormat()}\\{dateTime.GetRowKeyFormat()}");
            if (!await blobClient.ExistsAsync())
                //Since we already validated that the log had successful status in the controller
                // there should be a payload in the blob, if there isn't, it is an exceptional situation
                throw new Exception($"Something went wrong, cannot find log at {dateTime.GetRowKeyFormat()}");

            using MemoryStream memoryStream = new MemoryStream();
            await blobClient.DownloadToAsync(memoryStream);
            // Convert the MemoryStream to a string
            memoryStream.Position = 0;
            using StreamReader streamReader = new StreamReader(memoryStream, Encoding.UTF8);
            string content = await streamReader.ReadToEndAsync();
            return content;
        }
    }
}
