using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileStore.Persistence.Dapper.Migrations
{
    [Migration(202112122)]
    public class Migration_202112122 : Migration
    {
        public override void Down()
        {
            Alter.Table("File").AlterColumn("FullPath").AsString(255);
        }

        public override void Up()
        {
            Alter.Table("File").AlterColumn("FullPath").AsString(int.MaxValue);
        }
    }
}
