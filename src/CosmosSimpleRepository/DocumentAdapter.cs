using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System;

namespace CosmosSimpleRepository
{
    public class DocumentAdapter : IDocumentAdapter
    {
        readonly string _cosmosDbconnection;
        readonly string _databaseId;
        readonly string _cosmosEndpoint;
        readonly string _cosmosKey;

        IDocumentClient _client;

        public DocumentAdapter(string cosmosDbConnection, string databaseId, IStringUtils utils)
        {
            _databaseId = databaseId;
            _cosmosDbconnection = cosmosDbConnection;

            var connection = utils.ConvertConnectionString(cosmosDbConnection);

            if (!connection.TryGetValue("AccountEndpoint", out _cosmosEndpoint))
                throw new ArgumentException(nameof(cosmosDbConnection));

            if (!connection.TryGetValue("AccountKey", out _cosmosKey))
                throw new ArgumentException(nameof(cosmosDbConnection));

            if (string.IsNullOrWhiteSpace(databaseId))
                throw new ArgumentNullException(nameof(databaseId));
        }

        public string CosmosDbConnection => _cosmosDbconnection;

        public string CosmosEndpoint => _cosmosEndpoint;

        public string CosmosKey => _cosmosKey;

        public string DatabaseId => _databaseId;

        public IDocumentClient Client
        {
            get
            {
                if (_client == null)
                    _client = new DocumentClient(new Uri(_cosmosEndpoint), _cosmosKey);

                return _client;
            }
        }
    }
}
