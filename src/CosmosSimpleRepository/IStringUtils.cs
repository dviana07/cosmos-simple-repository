using System.Collections.Generic;

namespace CosmosSimpleRepository
{
    public interface IStringUtils
    {
        Dictionary<string, string> ConvertConnectionString(string cosmosDbConnection);
    }
}
