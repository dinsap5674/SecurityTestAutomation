using System.Collections.Generic;

namespace DAL.Model
{
    public class QueueMessage
    {
        public string Key { get; set; }
        public string Email { get; set; }
        public List<string> Attributes { get; set; }

    }
}
