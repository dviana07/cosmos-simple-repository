using System.Threading.Tasks;

namespace CosmosSimpleRepository
{
    public interface IDbInitializer
    {
        Task InitializeAsync();
    }
}
