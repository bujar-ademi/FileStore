using FileStore.Domain.Entities;
using System.Threading.Tasks;
using Dapper;
using FileStore.Application.Interfaces;
using FileStore.Application.Interfaces.Repository;
using System.Linq;
using System;

namespace FileStore.Persistence.Dapper
{
    public class FileRepository : Repository<File>, IFileRepository
    {

        public FileRepository(IContext context) : base(context, "File") { }

        public File GetFileByReference(Guid apiClientId, Guid reference)
        {
            using (var cn = Connection)
            {
                cn.Open();
                return cn.Query<File>(
                    @"SELECT * FROM [File] WHERE [Reference]=@Reference AND [MarkedForDeletion]=0 AND [ApiClientId]=@ApiClientId",
                    new { ApiClientId = apiClientId, Reference = reference }).First();
            }
        }

        public async Task<File> GetFileByReferenceAsync(Guid apiClientId, Guid reference)
        {
            using (var cn = Connection)
            {
                cn.Open();
                return await cn.QueryFirstAsync<File>(
                    @"SELECT * FROM [File] WHERE [Reference]=@Reference AND [MarkedForDeletion]=0 AND [ApiClientId]=@ApiClientId",
                    new { ApiClientId = apiClientId, Reference = reference });
            }
        }

        public new void Remove(File fileEntity)
        {
            using (var cn = Connection)
            {
                cn.Open();
                cn.Execute("DELETE FROM [File] WHERE Id=@Id", new { Id = fileEntity.Id });
            }
        }

        public new async Task RemoveAsync(File fileEntity)
        {
            using (var cn = Connection)
            {
                cn.Open();
                await cn.ExecuteAsync("DELETE FROM [File] WHERE Id=@Id", new { Id = fileEntity.Id });
            }
        }
    }
}
