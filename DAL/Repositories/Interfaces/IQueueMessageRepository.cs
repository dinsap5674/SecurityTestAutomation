using DAL.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories.Interfaces
{
    public interface IQueueMessageRepository
    {
        Task<int> InsertQueueMessage(QueueMessage message );
        Task<List<string>> GetAllAttributesPerEmailPerDay(string email);
    }
}
