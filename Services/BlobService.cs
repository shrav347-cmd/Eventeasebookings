using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace EventeaseBookingSystem.Services
{
    public class BlobService
    {
        private readonly string connectionString;
        private readonly string containerName = "venue-images";

        public BlobService(IConfiguration configuration)
        {
            connectionString = configuration.GetConnectionString("AzureBlobStorage")
                ?? throw new InvalidOperationException("AzureBlobStorage connection string is missing.");
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file was selected.");
            }

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();

            try
            {
                await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);
            }
            catch
            {
                // If Azure blocks public access, upload can still continue.
                // The image URL may not be publicly visible unless public blob access is enabled.
            }

            string fileExtension = Path.GetExtension(file.FileName);
            string fileName = $"{Guid.NewGuid()}{fileExtension}";

            var blobClient = containerClient.GetBlobClient(fileName);

            var blobHttpHeaders = new BlobHttpHeaders
            {
                ContentType = file.ContentType
            };

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, blobHttpHeaders);
            }

            return blobClient.Uri.ToString();
        }
    }
}