using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using DAL.Model;
using Azure.Storage.Queues;
using Newtonsoft.Json;
using TaskWebAPI.QueueService.Interfaces;
using BlobStorage.BlobService.Interfaces;

namespace TaskWebAPI.QueueService
{
    public class MessageQueueService : IMessageQueueService
    {
        private readonly ILogger<MessageQueueService> _logger;
        private readonly IConfiguration _configuration;
        private readonly IBlobService _blobService;

        public MessageQueueService(ILogger<MessageQueueService> logger, IConfiguration configuration, IBlobService blobService)
        {
            _configuration = configuration;
            _logger = logger;
            _blobService = blobService;
        }
        public async Task SendMessageToQueue(QueueMessage message)
        {
            string connectionString = _configuration["AzureWebJobsStorage"];

            QueueClient queue = new QueueClient(connectionString, _configuration["QueueName"]);
            string encodedMessage = Base64EncodeObject(message);
            await InsertMessageAsync(queue, encodedMessage);

            await _blobService.LogInformation(message.Email, $"Queue Message is send to QueueStorage");

            _logger.LogInformation($"Queue Message is send to QueueStorage");

        }
        static async Task InsertMessageAsync(QueueClient theQueue, string newMessage)
        {
            if (null != await theQueue.CreateIfNotExistsAsync())
            {
                Console.WriteLine("The queue was created.");
            }

            await theQueue.SendMessageAsync(newMessage);
        }
        public static string Base64EncodeObject(object obj)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj));
            return Convert.ToBase64String(plainTextBytes);
        }
    }
}
