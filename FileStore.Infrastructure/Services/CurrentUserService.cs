using FileStore.Application.Common.Exceptions;
using FileStore.Application.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;

namespace FileStore.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthenticationService authenticationService;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor,IAuthenticationService authenticationService)
        {
            _httpContextAccessor = httpContextAccessor;
            this.authenticationService = authenticationService;
        }

        public string ApiClientId
        {
            get
            {
                var apiClientId = _httpContextAccessor.HttpContext?.User?.FindFirstValue("uid");
                if (string.IsNullOrEmpty(apiClientId))
                {
                    // try to get if we have 
                    // token needs to be base64 (apiKey+secret)
                    if (_httpContextAccessor.HttpContext.Request?.Headers.TryGetValue("token", out Microsoft.Extensions.Primitives.StringValues value) == true) {
                        var decodedValue = Convert.FromBase64String(value);
                        var values = Encoding.UTF8.GetString(decodedValue).Split("+");
                        var apiClient = authenticationService.AuthenticateClient(values[0], values[1]);
                        if (apiClient == null)
                        {
                            throw new ForbiddenAccessException();
                        }
                        return apiClient.Id.ToString();
                    }
                    //if token doesnt exists maybe we have token in query parameter
                    if (_httpContextAccessor.HttpContext.Request?.Query.ContainsKey("token") == true)
                    {
                        if ( _httpContextAccessor.HttpContext.Request.Query.TryGetValue("token", out Microsoft.Extensions.Primitives.StringValues token) == true)
                        {
                            var decodedValue = Convert.FromBase64String(token);
                            var values = Encoding.UTF8.GetString(decodedValue).Split("+");
                            var apiClient = authenticationService.AuthenticateClient(values[0], values[1]);
                            if (apiClient == null)
                            {
                                throw new ForbiddenAccessException();
                            }
                            return apiClient.Id.ToString();
                        }
                    }
                    throw new ForbiddenAccessException();
                }
                return apiClientId;
            }
        }
    }
}
