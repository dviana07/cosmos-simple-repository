using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using System.Threading.Tasks;

namespace CosmosSimpleRepository
{
    public class DbInitializer : IDbInitializer
    {
        readonly IDocumentAdapter _doc;
        readonly string[] _collectionIds;

        public DbInitializer(
            IDocumentAdapter documentAdapter,
            string[] collectionIds)
        {
            _doc = documentAdapter;
            _collectionIds = collectionIds;
        }

        public async Task InitializeAsync()
        {
            await CreateDatabaseIfNotExistsAsync(_doc.DatabaseId).ConfigureAwait(false);

            foreach (var coll in _collectionIds)
            {
                await CreateCollectionIfNotExistsAsync(_doc.DatabaseId, coll);
            }
        }

        async Task CreateDatabaseIfNotExistsAsync(string databaseId)
        {
            try
            {
                await _doc.Client.ReadDatabaseAsync(
                        UriFactory.CreateDatabaseUri(databaseId))
                    .ConfigureAwait(false);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _doc.Client.CreateDatabaseAsync(
                            new Database { Id = databaseId })
                        .ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }
        }

        async Task CreateCollectionIfNotExistsAsync(string databaseId, string collectionId)
        {
            try
            {
                await _doc.Client.ReadDocumentCollectionAsync(
                        UriFactory.CreateDocumentCollectionUri(databaseId, collectionId))
                    .ConfigureAwait(false);
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    await _doc.Client.CreateDocumentCollectionAsync(
                            UriFactory.CreateDatabaseUri(databaseId),
                            new DocumentCollection { Id = collectionId },
                            new RequestOptions { OfferThroughput = 1000 })
                        .ConfigureAwait(false);
                }
                else
                {
                    throw;
                }
            }
        }
    }
}
