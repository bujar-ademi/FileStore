using FileStore.Domain.Entities;
using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileStore.Persistence.Dapper.Migrations
{
    [Migration(202112161)]
    public class Migration_202112161 : Migration
    {
        public override void Down()
        {
            throw new NotImplementedException();
        }

        public override void Up()
        {
            var apiClient = new ApiClient
            {
                Id = Guid.NewGuid(),
                Name = "5in5",
                ApiKey = "vzPGgNAq55",
                Secret = "detwXNhnFuy4Y2kbE3iUJrxfMa8Dq6SI",
                BasePath = "C:\\FileServer\\5in5\\",
                StorageType = ClientStorageType.File,
                StorageSettings = "5in5"
            };

            Insert.IntoTable("ApiClient").Row(new Dictionary<string, object>
            {
                {"Id", apiClient.Id },
                { "Name", apiClient.Name },
                { "ApiKey", apiClient.ApiKey },
                { "Secret", apiClient.Secret },
                { "BasePath", apiClient.BasePath },
                { "StorageType", (int)apiClient.StorageType },
                { "StorageSettings", apiClient.StorageSettings }
            });

            Insert.IntoTable("FileType").Row(new FileType
            {
                Id = Guid.NewGuid(),
                ApiClientId = apiClient.Id,
                Name = "Jpeg",
                ContentType = "image/jpeg",
                Allowed = true
            }).Row(new FileType
            {
                Id = Guid.NewGuid(),
                ApiClientId = apiClient.Id,
                Name = "Png",
                ContentType = "image/png",
                Allowed = true
            }).Row(new FileType
            {
                Id = Guid.NewGuid(),
                ApiClientId = apiClient.Id,
                Name = "Pdf",
                ContentType = "application/pdf",
                Allowed = true
            }).Row(new FileType
            {
                Id = Guid.NewGuid(),
                ApiClientId = apiClient.Id,
                Name = "Plain",
                ContentType = "text/plain",
                Allowed = true
            });
        }
    }
}
