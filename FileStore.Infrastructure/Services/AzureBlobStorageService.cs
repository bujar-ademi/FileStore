using Azure.Storage.Blobs;
using FileStore.Application.Common.Models;
using FileStore.Application.Interfaces.Repository;
using FileStore.Application.Interfaces.Services;
using FileStore.Domain.Entities;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using FileStore.Application.Helpers;
using FileStore.Application.Common.Exceptions;
using System.IO;
using System.Linq;
using FileStore.Application.Common.Models.Responses;

namespace FileStore.Infrastructure.Services
{
    public class AzureBlobStorageService : IStorageService
    {
        private readonly IApiClientRepository apiClientRepository;
        private readonly IFileRepository fileRepository;
        private readonly IFileTypeRepository fileTypeRepository;
        private readonly IHttpContextAccessor httpContextAccessor;
        public AzureBlobStorageService(IApiClientRepository apiClientRepository, IFileRepository fileRepository, IFileTypeRepository fileTypeRepository, IHttpContextAccessor httpContextAccessor)
        {
            this.apiClientRepository = apiClientRepository;
            this.fileRepository = fileRepository;
            this.fileTypeRepository = fileTypeRepository;
            this.httpContextAccessor = httpContextAccessor;
        }
        public async Task<bool> DeleteFileAsync(Guid fileReference, Guid apiClientId)
        {
            var apiClient = await apiClientRepository.GetByIdAsync(apiClientId);
            var blobSettings = apiClient.StorageSettings.Deserialize<AzureBlobSettings>();

            try
            {
                var fileEntity = await fileRepository.GetFileByReferenceAsync(apiClientId, fileReference);

                if (fileEntity == null)
                {
                    throw new NotFoundException("File does not exists");
                }

                var container = await GetContainer(blobSettings);
                BlobClient blob = container.GetBlobClient(fileEntity.Reference.ToString());
                var deleted = await blob.DeleteIfExistsAsync(Azure.Storage.Blobs.Models.DeleteSnapshotsOption.IncludeSnapshots);
                if (deleted)
                {
                    await fileRepository.RemoveAsync(fileEntity);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public async Task<FileModel> DownloadAsync(Guid apiClientId, Guid reference)
        {
            var apiClient = await apiClientRepository.GetByIdAsync(apiClientId);
            var blobSettings = apiClient.StorageSettings.Deserialize<AzureBlobSettings>();

            var fileEntity = await fileRepository.GetFileByReferenceAsync(apiClientId, reference);

            if (fileEntity == null)
            {
                throw new NotFoundException("File not found.");
            }
            var fileType = await fileTypeRepository.GetFileTypeByIdAsync(fileEntity.FileTypeId);
            if (fileType == null)
            {
                throw new NotFoundException("Uknown filetype");
            }

            var container = await GetContainer(blobSettings);
            BlobClient blob = container.GetBlobClient(fileEntity.Reference.ToString());

            try
            {
                Stream blobStream = new MemoryStream();
                await blob.DownloadToAsync(blobStream);

                var fileBytes = FileStorageService.ReadFully(blobStream);

                return new FileModel
                {
                    Reference = fileEntity.Reference,
                    FileBytes = fileBytes,
                    FileName = fileEntity.FileName,
                    ContentType = fileType.ContentType
                };
            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
                throw ex;
            }
        }

        public async Task<FileResponse> StoreFileAsync(ApiClient apiClient, IFormFile file)
        {
            var fileTypes = await fileTypeRepository.GetFileTypesByApiClientIdAsync(apiClient.Id);
            if (file.Length == 0)
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
                Size = file.Length,
                DateCreated = DateTime.UtcNow,
                FileTypeId = fileType.Id
            };

            var container = await GetContainer(apiClient.StorageSettings.Deserialize<AzureBlobSettings>());
            BlobClient blob = container.GetBlobClient(fileEntity.Reference.ToString());
            var response = await blob.UploadAsync(file.OpenReadStream());

            fileEntity.FullPath = blob.Uri.AbsoluteUri;

            await fileRepository.AddAsync(fileEntity);

            return new FileResponse
            {
                Reference = fileEntity.Reference,
                FileUrl = FileStorageService.GetFilePublicUrl(httpContextAccessor, fileEntity.Reference),
                FileName = fileEntity.FileName
            };
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

            var container = await GetContainer(apiClient.StorageSettings.Deserialize<AzureBlobSettings>());
            BlobClient blob = container.GetBlobClient(fileEntity.Reference.ToString());

            var memoryStream = new MemoryStream(file.FileBytes);
            var response = await blob.UploadAsync(memoryStream);

            fileEntity.FullPath = blob.Uri.AbsoluteUri;

            await fileRepository.AddAsync(fileEntity);

            return new FileResponse
            {
                Reference = fileEntity.Reference,
                FileName = fileEntity.FileName,
                FileUrl = FileStorageService.GetFilePublicUrl(httpContextAccessor, fileEntity.Reference)
            };
        }

        private async Task<BlobContainerClient> GetContainer(AzureBlobSettings azureBlobSettings)
        {
            BlobContainerClient container = new BlobContainerClient(azureBlobSettings.ConnectionString, azureBlobSettings.ContainerName);
            await container.CreateIfNotExistsAsync(Azure.Storage.Blobs.Models.PublicAccessType.BlobContainer);

            return container;
        }
    }
}
