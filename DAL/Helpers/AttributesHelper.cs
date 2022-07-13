using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Helpers
{
    public static class AttributesHelper
    {
        public static async Task<List<string>> PrepareListOfAttributes(DbDataReader dbDataReader)
        {
            List<string> totalAttributes = new List<string>();
            while(await dbDataReader.ReadAsync().ConfigureAwait(false))
            {
                List<string> firstRowAttributes = new List<string>();
                if (!dbDataReader.IsDBNull(0))
                {
                    firstRowAttributes = JsonConvert.DeserializeObject<List<string>>(dbDataReader.GetString(0));
                }
                totalAttributes.AddRange(firstRowAttributes);
            }
            return totalAttributes;
        }
    }
}
