using FileStore.Domain.Entities;
using System.Threading.Tasks;
using Dapper;
using FileStore.Application.Interfaces;
using FileStore.Application.Interfaces.Repository;
using System.Linq;

namespace FileStore.Persistence.Dapper
{
    public class ApiClientRepository : Repository<ApiClient>, IApiClientRepository
    {
        public ApiClientRepository(IContext context) : base(context, "ApiClient") { }

        public ApiClient GetApiClientByApiKey(string apiKey)
        {
            using (var cn = Connection)
            {
                cn.Open();
                return cn.Query<ApiClient>("SELECT * FROM APICLIENT WHERE apiKey=@apiKey", new { apiKey })
                    .SingleOrDefault();
            }
        }

        public async Task<ApiClient> GetApiClientByApiKeyAsync(string apiKey)
        {
            using (var cn = Connection)
            {
                cn.Open();
                return await cn.QueryFirstOrDefaultAsync<ApiClient>("SELECT * FROM APICLIENT WHERE apiKey=@apiKey", new { apiKey });
            }
        }
    }
}
