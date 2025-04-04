using FileStore.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace FileStore.Application.Interfaces.Repository
{
    public interface IFileRepository : IRepository<File>
    {
        File GetFileByReference(Guid apiClientId, Guid reference);
        Task<File> GetFileByReferenceAsync(Guid apiClientId, Guid reference);
    }
}
