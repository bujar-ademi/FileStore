using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileStore.Persistence.Dapper.Migrations
{
    [Migration(202112131)]
    public class Migration_202112131 : Migration
    {
        public override void Down()
        {
            Delete.Column("StorageType").FromTable("ApiClient");
            Delete.Column("StorageSettings").FromTable("ApiClient");
        }

        public override void Up()
        {
            Alter.Table("ApiClient").AddColumn("StorageType").AsInt16().WithDefaultValue(0);
            Alter.Table("ApiClient").AddColumn("StorageSettings").AsString(int.MaxValue);
        }
    }
}
