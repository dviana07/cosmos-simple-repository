using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace CosmosSimpleRepository
{
    public interface IRepository<T> where T : class
    {
        Task<T> GetItemAsync(string id);

        Task<IEnumerable<T>> GetItemsAsync(Expression<Func<T, bool>> predicate);

        Task<T> CreateItemAsync(T item);

        Task<T> UpdateItemAsync(string id, T item);

        Task DeleteItemAsync(string id);
    }
}
