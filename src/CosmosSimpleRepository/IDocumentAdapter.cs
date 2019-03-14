using Microsoft.Azure.Documents;

namespace CosmosSimpleRepository
{
    public interface IDocumentAdapter
    {
        IDocumentClient Client { get; }
        string CosmosDbConnection { get; }
        string CosmosEndpoint { get; }
        string CosmosKey { get; }
        string DatabaseId { get; }

    }
}
