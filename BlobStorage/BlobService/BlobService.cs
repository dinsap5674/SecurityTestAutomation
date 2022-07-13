using BlobStorage.BlobService.Helper;
using BlobStorage.BlobService.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace BlobStorage.BlobService
{
    public class BlobService : IBlobService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<BlobService> _logger;

        public BlobService(IConfiguration configuration, ILogger<BlobService> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task LogError(string email, string logMessage)
        {
            logMessage = (DateTime.UtcNow).ToLongTimeString() + " Error : " + logMessage + Environment.NewLine;
            await WriteLog(email, logMessage).ConfigureAwait(false);
        }

        public async Task LogInformation(string email, string logMessage)
        {
            logMessage = (DateTime.UtcNow).ToLongTimeString() + " Information : " + logMessage + Environment.NewLine;
            await WriteLog(email, logMessage).ConfigureAwait(false);
        }

        public async Task LogWarning(string email, string logMessage)
        {
            logMessage = (DateTime.UtcNow).ToLongTimeString() + " Warning : " + logMessage + Environment.NewLine;
            await WriteLog(email, logMessage).ConfigureAwait(false);
        }

        public async Task WriteLog(string email, string logMessage)
        {
            //GenerateFileName 
            var fileName = HashingHelper.GenerateFileName(email);

            //_logger.LogInformation($"Generated File name: {fileName}");

            CloudStorageAccount storage = CloudStorageAccount.Parse(_configuration["AzureWebJobsStorage"]);
            CloudBlobClient client = storage.CreateCloudBlobClient();
            CloudBlobContainer container = client.GetContainerReference(_configuration["containername"]);
            CloudAppendBlob blob = container.GetAppendBlobReference(fileName);

            bool isPresent = await blob.ExistsAsync();
            if (!isPresent)
            {
                await blob.CreateOrReplaceAsync().ConfigureAwait(false);
            }
            //await blob.AppendTextAsync(logMessage);

            var bytes = Encoding.UTF8.GetBytes(logMessage);

            var stream = new MemoryStream(bytes);

            await blob.AppendBlockAsync(stream);

        }
    }
}
