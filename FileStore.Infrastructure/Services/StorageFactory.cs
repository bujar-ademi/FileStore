using FileStore.Application.Common.Exceptions;
using FileStore.Application.Interfaces.Services;
using FileStore.Domain.Entities;
using System;

namespace FileStore.Infrastructure.Services
{
    public class StorageFactory : IStorageFactory
    {
        private readonly IServiceProvider serviceProvider;

        public StorageFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IStorageService GetStorageService(ApiClient client)
        {
            switch (client.StorageType)
            {
                case ClientStorageType.File:
                    return (IStorageService)serviceProvider.GetService(typeof(FileStorageService));
                case ClientStorageType.AzureBlobStorage:
                    return (IStorageService)serviceProvider.GetService(typeof(AzureBlobStorageService));
                default:
                    throw new ConflictException("Uknown storage type");
            }
        }
    }
}
