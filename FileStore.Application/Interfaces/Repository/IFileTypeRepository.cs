using FileStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileStore.Application.Interfaces.Repository
{
    public interface IFileTypeRepository : IRepository<FileType>
    {
        Task<FileType> GetFileTypeByContentTypeAsync(Guid apiClientId, string contentType);
        Task<List<FileType>> GetFileTypesByApiClientIdAsync(Guid apiClientId);
        Task<FileType> GetFileTypeByIdAsync(Guid fileTypeId);
    }
}
