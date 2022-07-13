using System.Threading.Tasks;
using DAL.Model;

namespace TaskWebAPI.QueueService.Interfaces
{
    public interface IMessageQueueService
    {
        Task SendMessageToQueue(QueueMessage message);
    }
}
