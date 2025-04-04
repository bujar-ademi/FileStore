using FileStore.Application.Interfaces;
using FileStore.Application.Interfaces.Repository;
using FluentMigrator.Runner;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace FileStore.Persistence.Dapper
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDapper(this IServiceCollection services, IConfiguration configuration)
        {
            // repositories
            services.AddTransient<IFileRepository, FileRepository>();
            services.AddTransient<IFileTypeRepository, FileTypeRepository>();
            services.AddTransient<IApiClientRepository, ApiClientRepository>();

            // context
            var context = new Context(configuration.GetConnectionString("FileStoreDb"));
            services.AddSingleton<IContext>(context);

            services.AddFluentMigratorCore()
                .ConfigureRunner(c => c.AddSqlServer2016()
                    .WithGlobalConnectionString(configuration.GetConnectionString("FileStoreDb"))
                    .ScanIn(Assembly.GetExecutingAssembly()).For.Migrations());

            return services;
        }

        public static IHost MigrateDatabase(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var migrationService = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
                try
                {
                    migrationService.ListMigrations();
                    migrationService.MigrateUp();
                }
                catch
                {
                    //log errors or ...
                    throw;
                }
            }
            return host;
        }
    }
}
