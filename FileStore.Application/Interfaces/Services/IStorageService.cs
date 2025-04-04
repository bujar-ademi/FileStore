using FileStore.Application.Common.Models;
using FileStore.Application.Common.Models.Responses;
using FileStore.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileStore.Application.Interfaces.Services
{
    public interface IStorageService
    {
        Task<FileResponse> StoreFileAsync(ApiClient apiClient, IFormFile file);
        Task<FileResponse> StoreFileAsync(ApiClient apiClient, FileModel file);
        Task<bool> DeleteFileAsync(Guid fileReference, Guid apiClientId);
        Task<FileModel> DownloadAsync(Guid apiClientId, Guid reference);
    }
}
