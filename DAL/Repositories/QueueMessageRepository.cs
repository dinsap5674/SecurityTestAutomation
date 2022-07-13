using DAL.Helpers;
using DAL.Model;
using DAL.Repositories.Interfaces;
using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using BlobStorage.BlobService.Interfaces;

namespace DAL.Repositories
{
    public class QueueMessageRepository : IQueueMessageRepository
    {
        private readonly IConfiguration _confg;
        private readonly ILogger<QueueMessageRepository> _logger;
        private readonly IBlobService _blobService;
        public QueueMessageRepository(IConfiguration configuration, ILogger<QueueMessageRepository> logger, IBlobService blobService)
        {
            _confg = configuration;
            _logger = logger;
            _blobService = blobService;
        }
        public IDbConnection Connection
        {
            get
            {
                return new SqlConnection(_confg.GetConnectionString("TestDB"));
            }
        }

        public async Task<List<string>> GetAllAttributesPerEmailPerDay(string email)
        {
            string sQuery = "SELECT Attributes FROM MessageQueue WHERE Date = @currentUtcDate AND Email = @email";

            using (SqlConnection con = (SqlConnection)Connection)
            {
                var dbDataReader = await con.ExecuteReaderAsync(sQuery, new { currentUtcDate = (DateTime.UtcNow).Date, email} ).ConfigureAwait(false);
                //To get the data obtained from the hybrid table and cast it in the desired format.
                var attributesList = await AttributesHelper.PrepareListOfAttributes(dbDataReader).ConfigureAwait(false);

                await _blobService.LogInformation(email, "QueueMessage repository is called to get all attributes per email per day.");

                _logger.LogInformation("QueueMessage repository is called to get all attributes per email per day.");

                return attributesList;
            }
        }

        public async Task<int> InsertQueueMessage(QueueMessage message)
        {
            string sQuery = "INSERT INTO MessageQueue ([Key], Email, Attributes) " +
                    "VALUES (@Key, @Email, @Attributes)";
            using (IDbConnection con = Connection)
            {
                var result = await con.ExecuteAsync(sQuery, new { Key = message.Key, Email = message.Email, Attributes = JsonConvert.SerializeObject(message.Attributes) }).ConfigureAwait(false);

                await _blobService.LogInformation(message.Email, "Queue Message repository is called to insert the message !");

                _logger.LogInformation("Queue Message repository is called to insert the message !");
                return result;
            }
        }
    }
}
