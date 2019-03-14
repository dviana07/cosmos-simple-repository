using System;
using System.Linq;
using System.Collections.Generic;

namespace CosmosSimpleRepository
{
    public class StringUtils : IStringUtils
    {
        public Dictionary<string, string> ConvertConnectionString(string cosmosDbConnection)
        {
            Dictionary<string, string> ret = null;

            if (!string.IsNullOrWhiteSpace(cosmosDbConnection))
            {
                ret = cosmosDbConnection
                    .Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Split(new char[] { '=' }, 2, StringSplitOptions.RemoveEmptyEntries))
                    .Where(kvp => kvp.Length == 2)
                    ?.ToDictionary(k => k[0], v => v[1], StringComparer.InvariantCultureIgnoreCase);
            }

            return ret ?? new Dictionary<string, string>();
        }
    }
}
