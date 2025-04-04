using FileStore.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileStore.Domain.Entities
{
    public class ApiClient : EntityBase
    {
        public string Name { get; set; }
        public string ApiKey { get; set; }
        public string Secret { get; set; }
        public string BasePath { get; set; }
        public ClientStorageType StorageType { get; set; }
        public string StorageSettings { get; set; }
    }

    public enum ClientStorageType : Int16
    {
        File,
        AzureBlobStorage,
        AWS
    }
}
