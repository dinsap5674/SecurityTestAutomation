using BlobStorage.BlobService.Interfaces;
using DAL.Model;
using DAL.Repositories.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using QueueReceiver.Helpers;
using System.Text;
using System.Threading.Tasks;

namespace QueueReceiver
{
    public class Functions
    {
        private readonly ILogger<Functions> _logger;
        private readonly IQueueMessageRepository _queueMessageRepository;
        private readonly IConfiguration _configuration;
        private readonly IBlobService _blobService;
  
        public Functions(IQueueMessageRepository queueMessageRepository, ILogger<Functions> logger, IConfiguration configuration, IBlobService blobService)
        {
            _logger = logger;
            _queueMessageRepository = queueMessageRepository;
            _configuration = configuration;
            _blobService = blobService;
        }
        //To get the queue name from appsetting.json
        public async Task ProcessQueueMessage([QueueTrigger("%queuename%")] byte[] messageBytes)
        {
            string decodeMessage = Encoding.UTF8.GetString(messageBytes);
            var messageQueue = Base64DecodeObject(decodeMessage);
            
            //Get all the attributes per email per day
            var existingAttributesList = await _queueMessageRepository.GetAllAttributesPerEmailPerDay(messageQueue.Email).ConfigureAwait(false);
            var (duplicateExists, attributeCount, attributesList) = QueueMessageHelper.CheckAttributesDuplicateAndCount(messageQueue.Attributes, existingAttributesList);

            if (!duplicateExists)
            {
                //Store the QueueMessage if there is not any duplicate attributes
                var result = await _queueMessageRepository.InsertQueueMessage(messageQueue);
                if (result > 0)
                {
                    _logger.LogInformation("QueueMessage is Successfully stored in the sql table.");

                    await _blobService.LogInformation(messageQueue.Email, "QueueMessage is Successfully stored in the sql table.");
                    
                    //Send Email if the attrubutes counts => 10
                    if (attributeCount == 10)
                    {
                        await SendEmailHelper.SendEmail(messageQueue.Email, attributesList, _configuration, _logger, _blobService);
                    }
                }
            }
            else
            {
                _logger.LogWarning($"QueueMessage contains duplicate attributes.");

                await _blobService.LogWarning(messageQueue.Email, "QueueMessage contains duplicate attributes.");
            }
        }
        public static QueueMessage Base64DecodeObject(string base64String)
        {
            return JsonConvert.DeserializeObject<QueueMessage>(base64String);
        }
    }
}
