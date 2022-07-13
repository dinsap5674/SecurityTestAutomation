using BlobStorage.BlobService.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace QueueReceiver.Helpers
{
    public static class SendEmailHelper
    {
        public static async Task SendEmail(string email, List<string> attributes, IConfiguration configuration, ILogger logger, IBlobService blobService)
        {
            var apiKey = configuration["SENDGRID_API_KEY"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(configuration["SenderEmail"], "Example User");
            var subject = "Congratulations for sending some unique attribute";
            var to = new EmailAddress(email, "Example User");
            //var plainTextContent = "and easy to do anywhere, even with C#";
            var plainTextContent = JsonConvert.SerializeObject(attributes);
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
            if (response.IsSuccessStatusCode)
            {
                logger.LogInformation("Email has be send to the recepient successfully.");
                await blobService.LogInformation(email, "Email has be send to the recepient successfully.");
                
            }
        }
    }
}
