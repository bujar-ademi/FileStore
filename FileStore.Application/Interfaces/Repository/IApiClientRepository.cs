using FileStore.Domain.Entities;
using System.Threading.Tasks;

namespace FileStore.Application.Interfaces.Repository
{
    public interface IApiClientRepository : IRepository<ApiClient>
    {
        ApiClient GetApiClientByApiKey(string apiKey);
        Task<ApiClient> GetApiClientByApiKeyAsync(string apiKey);
    }
}
