using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlobStorage.BlobService.Interfaces
{
    public interface IBlobService
    {
        Task LogInformation(string email, string logMessage);
        Task LogWarning(string email, string logMessage);
        Task LogError(string email, string logMessage);

    }
}
