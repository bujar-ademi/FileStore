using FileStore.Application.Common.Exceptions;
using FileStore.Application.Common.Models;
using FileStore.Application.Common.Models.Responses;
using FileStore.Application.Interfaces.Repository;
using FileStore.Application.Interfaces.Services;
using FileStore.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStore.Infrastructure.Services
{
    public class FileStorageService : IStorageService
    {
        private readonly IApiClientRepository apiClientRepository;
        private readonly IFileRepository fileRepository;
        private readonly IFileTypeRepository fileTypeRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public FileStorageService(IApiClientRepository apiClientRepository, IFileRepository fileRepository, IFileTypeRepository fileTypeRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.apiClientRepository = apiClientRepository;
            this.fileRepository = fileRepository;
            this.fileTypeRepository = fileTypeRepository;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<bool> DeleteFileAsync(Guid fileReference, Guid apiClientId)
        {
            var file = await fileRepository.GetFileByReferenceAsync(apiClientId, fileReference);

            if (file == null)
            {
                throw new NotFoundException("File does not exists");
            }
            var fullPath = file.FullPath;
            try
            {
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                    await fileRepository.RemoveAsync(file);
                    return true;
                } else
                {
                    return false;
                }
            } catch (Exception)
            {
                return false;
            }
        }

        public async Task<FileModel> DownloadAsync(Guid apiClientId, Guid reference)
        {
            var file = await fileRepository.GetFileByReferenceAsync(apiClientId, reference);

            if (file == null)
            {
                throw new NotFoundException("File not found.");
            }
            var fileType = await fileTypeRepository.GetFileTypeByIdAsync(file.FileTypeId);
            if (fileType == null)
            {
                throw new NotFoundException("Uknown filetype");
            }

            var fileBytes = await System.IO.File.ReadAllBytesAsync(file.FullPath);

            return new FileModel
            {
                FileBytes = fileBytes,
                FileName = file.FileName,
                ContentType = fileType.ContentType,
                Reference = file.Reference,
                DateCreated = file.DateCreated
            };
        }

        public async Task<FileResponse> StoreFileAsync(ApiClient apiClient, IFormFile file)
        {
            byte[] fileBytes;

            var filePath = Path.GetTempFileName();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                stream.Position = 0;
                await file.CopyToAsync(stream);
                fileBytes = ReadFully(stream);
            }

            var fileModel = new FileModel
            {
                FileBytes = fileBytes,
                FileName = file.FileName,
                ContentType = file.ContentType
            };

            return await StoreFileAsync(apiClient, fileModel);
        }       

        public async Task<FileResponse> StoreFileAsync(ApiClient apiClient, FileModel file)
        {
            var fileTypes = await fileTypeRepository.GetFileTypesByApiClientIdAsync(apiClient.Id);
            if (file.FileBytes.Length == 0)
            {
                throw new ConflictException("Empty file");
            }
            var fileType = fileTypes.FirstOrDefault(x => x.ContentType == file.ContentType);
            if (fileType == null || !fileType.Allowed)
            {
                throw new ConflictException("Invalid ContentType");
            }

            var fileEntity = new Domain.Entities.File
            {
                Id = Guid.NewGuid(),
                FileName = file.FileName,
                Reference = Guid.NewGuid(),
                ApiClientId = apiClient.Id,
                Size = file.FileBytes.Length,
                DateCreated = DateTime.UtcNow,
                FileTypeId = fileType.Id
            };

            fileEntity = SaveFileToDisk(file.FileBytes, fileEntity, apiClient);
            await fileRepository.AddAsync(fileEntity);

            return new FileResponse
            {
                Reference = fileEntity.Reference,
                FileName = fileEntity.FileName,
                FileUrl = GetFilePublicUrl(httpContextAccessor, fileEntity.Reference)
            };
        }

        private Domain.Entities.File SaveFileToDisk(byte[] fileBytes, Domain.Entities.File file, ApiClient apiClient)
        {
            var fullPath = apiClient.BasePath + '\\' + Guid.NewGuid();
            System.IO.File.WriteAllBytes(fullPath, fileBytes);
            file.FullPath = fullPath;

            return file;
        }

        public static byte[] ReadFully(Stream input)
        {
            input.Position = 0;
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0) ms.Write(buffer, 0, read);
                return ms.ToArray();
            }
        }

        public static string GetFilePublicUrl(IHttpContextAccessor httpContextAccessor, Guid reference)
        {
            var hostUrl = httpContextAccessor.HttpContext.Request.Host.Value;
            if (!hostUrl.StartsWith("https://"))
            {
                hostUrl = "https://" + hostUrl;
            }
            return $"{hostUrl}/api/file/{reference}";
        }
    }
}
