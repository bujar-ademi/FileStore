using FileStore.Domain.Entities;
using FluentMigrator;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileStore.Persistence.Dapper.Migrations
{
    [Migration(202112241)]
    public class Migration_20211224_add_distributedCache : Migration
    {
        public override void Down()
        {
            
        }

        public override void Up()
        {
            Execute.Sql(@"CREATE TABLE [dbo].[DistributedCache](
	            [Id] [nvarchar](449) NOT NULL,
	            [Value] [varbinary](max) NOT NULL,
	            [ExpiresAtTime] [datetimeoffset](7) NOT NULL,
	            [SlidingExpirationInSeconds] [bigint] NULL,
	            [AbsoluteExpiration] [datetimeoffset](7) NULL,
             CONSTRAINT [PK_DistributedCache] PRIMARY KEY CLUSTERED 
            (
	            [Id] ASC
            )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
            ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
            GO");
        }
    }
}
