using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using NewLifeHRT.Application.Services.Interfaces;
using NewLifeHRT.Infrastructure.Settings;

namespace NewLifeHRT.Application.Services.Services
{
    public class BlobService : IBlobService
    {
        private readonly AzureBlobStorageSettings _azureBlobStorageSettings;

        public BlobService(IOptions<AzureBlobStorageSettings> azureBlobStorageSettings)
        {
            _azureBlobStorageSettings = azureBlobStorageSettings.Value;
        }

        private string BuildBlobUrl(string blobPath)
        {
            var containerUrl = _azureBlobStorageSettings.ContainerSasUrl?.TrimEnd('/');
            var sasToken = _azureBlobStorageSettings.SasToken?.TrimStart('?');
            return $"{containerUrl}/{blobPath}?{sasToken}";
        }

        public async Task<string> UploadFileAsync(IFormFile file, string blobPath)
        {
            try
            {
                string uploadUrl = BuildBlobUrl(blobPath);
                BlobClient blobClient = new BlobClient(new Uri(uploadUrl));
                using (var stream = file.OpenReadStream())
                {
                    await blobClient.UploadAsync(stream, overwrite: true);
                }
                return blobClient.Uri.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<string> UploadMediaAsync(byte[] mediaBytes, string blobPath, string contentType)
        {
            try
            {
                string uploadUrl = BuildBlobUrl(blobPath);
                BlobClient blobClient = new BlobClient(new Uri(uploadUrl));
                using var stream = new MemoryStream(mediaBytes);
                var options = new BlobUploadOptions
                {
                    HttpHeaders = new BlobHttpHeaders { ContentType = contentType }
                };
                await blobClient.UploadAsync(stream, options);
                return blobClient.Uri.ToString();
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
