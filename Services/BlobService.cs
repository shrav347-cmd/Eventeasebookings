using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;

namespace EventeaseBookingSystem.Services
{
    public class BlobService
    {
        private readonly string connectionString = "UseDevelopmentStorage=true";
        private readonly string containerName = "venue-images";

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("No file was selected.");
            }

            var blobServiceClient = new BlobServiceClient(connectionString);
            var containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();

            await containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);

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