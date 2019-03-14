using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.Documents.Linq;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Threading.Tasks;

namespace CosmosSimpleRepository
{
    public abstract class Repository<T> : IRepository<T>
        where T : class
    {
        protected Repository(string collectionId, IDocumentAdapter documentAdapter)
        {
            CollectionId = collectionId;
            Doc = documentAdapter;
        }

        protected IDocumentAdapter Doc;

        protected IDocumentClient Client => Doc.Client;

        protected string CollectionId { get; }

        public virtual async Task<T> GetItemAsync(string id)
        {
            try
            {
                Document document = await Client.ReadDocumentAsync(
                        UriFactory.CreateDocumentUri(Doc.DatabaseId, CollectionId, id)
                    ).ConfigureAwait(false);

                return (T)(dynamic)document;
            }
            catch (DocumentClientException e)
            {
                if (e.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate)
        {
            IDocumentQuery<T> query =
                Client.CreateDocumentQuery<T>(
                        UriFactory.CreateDocumentCollectionUri(Doc.DatabaseId, CollectionId),
                        new FeedOptions { MaxItemCount = -1 })
                    .Where(predicate)
                    .AsDocumentQuery();

            var results = new List<T>();

            while (query.HasMoreResults)
            {
                results.AddRange(await query.ExecuteNextAsync<T>().ConfigureAwait(false));
            }

            return results;
        }

        public virtual async Task<T> CreateItemAsync(T item)
        {
            var document = await Client.CreateDocumentAsync(
                    UriFactory.CreateDocumentCollectionUri(Doc.DatabaseId, CollectionId),
                    item)
                .ConfigureAwait(false);

            return (T)(dynamic)document.Resource;
        }

        public virtual async Task<T> UpdateItemAsync(string id, T item)
        {
            var document = await Client.ReplaceDocumentAsync(
                    UriFactory.CreateDocumentUri(Doc.DatabaseId, CollectionId, id),
                    item
                ).ConfigureAwait(false);

            return (T)(dynamic)document;
        }

        public virtual async Task DeleteItemAsync(string id)
        {
            var document = await Client.DeleteDocumentAsync(
                    UriFactory.CreateDocumentUri(Doc.DatabaseId, CollectionId, id)
                ).ConfigureAwait(false);
        }
    }
}
