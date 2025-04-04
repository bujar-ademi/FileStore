using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileStore.Persistence.Dapper.Migrations
{
    [Migration(20211212)]
    public class InitializeTables_20211212 : Migration
    {
        public override void Down()
        {
            Delete.Table("File");
            Delete.Table("FileType");
            Delete.Table("ApiClient");
        }

        public override void Up()
        {
            Create.Table("ApiClient")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Name").AsString(50).NotNullable()
                .WithColumn("ApiKey").AsString(50).NotNullable()
                .WithColumn("Secret").AsString(128).NotNullable()
                .WithColumn("BasePath").AsString(255);

            Create.Table("FileType")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("ApiClientId").AsGuid().NotNullable().ForeignKey("ApiClient", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("Name").AsString(100)
                .WithColumn("ContentType").AsString(128)
                .WithColumn("Allowed").AsBoolean().WithDefaultValue(true);

            Create.Table("File")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("Reference").AsGuid().NotNullable().Unique()
                .WithColumn("FileTypeId").AsGuid().ForeignKey("FileType", "Id").OnDelete(System.Data.Rule.None)
                .WithColumn("ApiClientId").AsGuid().NotNullable().ForeignKey("ApiClient", "Id").OnDelete(System.Data.Rule.Cascade)
                .WithColumn("FileName").AsString(255)
                .WithColumn("FullPath").AsString()
                .WithColumn("Size").AsInt64()
                .WithColumn("DateCreated").AsDateTime().WithDefaultValue(DateTime.Now)
                .WithColumn("MarkedForDeletion").AsBoolean().WithDefaultValue(false);

        }
    }
}
