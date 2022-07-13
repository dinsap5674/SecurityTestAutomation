using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueueReceiver.Helpers
{
    public static class QueueMessageHelper
    {
        public static (bool duplicateExists, int attributeCount, List<string> attributesList) CheckAttributesDuplicateAndCount(List<string> receivedAttributes, List<string> existingAttributes)
        {
            //All the attributes are added to the same list of string
            existingAttributes.AddRange(receivedAttributes);

            //Check if an email contains duplicate attributes.
            if (!existingAttributes.GroupBy(n => n).Any(c => c.Count() > 1))
            {
                return (false, existingAttributes.Count(), existingAttributes);
            }
            return (true, 0, existingAttributes);
        }
    }
}
