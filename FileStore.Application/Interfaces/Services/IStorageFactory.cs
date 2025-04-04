using FileStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileStore.Application.Interfaces.Services
{
    public interface IStorageFactory
    {
        IStorageService GetStorageService(ApiClient client);
    }
}
