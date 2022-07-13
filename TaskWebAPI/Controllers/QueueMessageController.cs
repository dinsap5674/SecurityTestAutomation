using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Threading.Tasks;
using DAL.Model;
using TaskWebAPI.QueueService.Interfaces;
using System.Linq;
using BlobStorage.BlobService.Interfaces;

namespace TaskWebAPI.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class QueueMessageController : Controller
    {
        private readonly IMessageQueueService _messageQueueService;
        private readonly IBlobService _blobService;

        public QueueMessageController(IMessageQueueService messageQueueService, IBlobService blobService)
        {
            _messageQueueService = messageQueueService;
            _blobService = blobService;
        }
        /// <summary>
        /// Asynchronous post request.(Internal System Specific Request)
        /// </summary>
        /// <remarks>To receive a request to process it.
        /// </remarks>
        // POST TaskMessage
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(string))]
        public async Task<IActionResult> TaskMessage([FromBody] QueueMessage message)
        {
            //_logger.LogInformation($"Received Json {JsonSerializer.Serialize(message)}.");

            //Check if an email contains duplicate attributes.
            if(!message.Attributes.GroupBy(n =>n).Any(c => c.Count() > 1))
            {
                //Blob service is called to log the messages in storage account
                await _blobService.LogInformation(message.Email, "Queue message is received.");

                await _messageQueueService.SendMessageToQueue(message);

                return Ok("Received request is send to the Queue Successfully.");
            }
            await _blobService.LogWarning(message.Email, "Queue message contains duplicate attributes");

            return Ok("Requests with duplicate attributes are not allowed");
        }
    }
}
