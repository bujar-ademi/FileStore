using FileStore.Application.Common.Models;
using FileStore.Application.Interfaces;
using FileStore.Application.Interfaces.Services;
using FileStore.Infrastructure.Providers;
using FileStore.Infrastructure.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileStore.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<ICacheProvider, CacheProvider>();

            services.AddTransient<IAuthenticationService, AuthenticationService>();
            services.AddTransient<IStorageFactory, StorageFactory>();

            services.AddScoped<FileStorageService>()
                .AddScoped<IStorageService, FileStorageService>(s => s.GetService<FileStorageService>());

            services.AddScoped<AzureBlobStorageService>()
                .AddScoped<IStorageService, AzureBlobStorageService>(s => s.GetService<AzureBlobStorageService>());

            services.Configure<JWTSettings>(configuration.GetSection("JWTSettings"));

            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = configuration.GetConnectionString("FileStoreDb");
                options.TableName = "DistributedCache";
                options.SchemaName = "dbo";
            });

            return services;
        }
    }
}
