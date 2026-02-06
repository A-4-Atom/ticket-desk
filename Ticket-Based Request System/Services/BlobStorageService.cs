using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;

namespace Ticket_Based_Request_System.Services
{
    public class BlobStorageService
    {
        private readonly BlobContainerClient _container;

        public BlobStorageService(IConfiguration configuration)
        {
            try
            {
                var blobServiceClient = new BlobServiceClient(
                    configuration["BlobStorage:ConnectionString"]
                );

                _container = blobServiceClient.GetBlobContainerClient(
                    configuration["BlobStorage:ContainerName"]
                );
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> UploadAsync(
            string blobPath,
            Stream fileStream,
            string contentType)
        {
            try
            {
                var blobClient = _container.GetBlobClient(blobPath);
                await blobClient.UploadAsync(fileStream, overwrite: true);
                return blobClient.Uri.ToString();
            }
            catch
            {
                throw;
            }
        }
    }
}
