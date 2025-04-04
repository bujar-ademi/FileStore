using FileStore.Application.Interfaces;
using FileStore.Application.Interfaces.Repository;
using FileStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace FileStore.Persistence.Dapper
{
    public class FileTypeRepository : Repository<FileType>, IFileTypeRepository
    {
        public FileTypeRepository(IContext context) : base(context, "FileType") { }

        public async Task<FileType> GetFileTypeByContentTypeAsync(Guid apiClientId, string contentType)
        {
            using (var cn = Connection)
            {
                cn.Open();
                return await cn.QueryFirstOrDefaultAsync<FileType>(
                    "SELECT * FROM FileType WHERE apiClientId=@apiClientId AND ContentType=@ContentType",
                    new { apiClientId, ContentType = contentType });
            }
        }

        public async Task<FileType> GetFileTypeByIdAsync(Guid fileTypeId)
        {
            using (var cn = Connection)
            {
                cn.Open();
                return await cn.QueryFirstOrDefaultAsync<FileType>("SELECT * FROM FileType WHERE id=@fileTypeId", new { fileTypeId });
            }
        }

        public async Task<List<FileType>> GetFileTypesByApiClientIdAsync(Guid apiClientId)
        {
            using (var cn = Connection)
            {
                cn.Open();
                return (await cn.QueryAsync<FileType>("SELECT * FROM FileType WHERE apiClientId=@apiClientId", new { apiClientId }))
                    .ToList();
            }
        }
    }
}
