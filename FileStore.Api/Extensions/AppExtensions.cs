﻿using FileStore.Api.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace FileStore.Api.Extensions
{
    public static class AppExtensions
    {
        public static void UseErrorHandlingMiddleware(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHandlerMiddleware>();
        }

    }
}
