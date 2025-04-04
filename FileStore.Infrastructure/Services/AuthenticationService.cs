using FileStore.Application.Common.Models;
using FileStore.Application.Interfaces;
using FileStore.Application.Interfaces.Repository;
using FileStore.Application.Interfaces.Services;
using FileStore.Domain.Entities;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace FileStore.Infrastructure.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IApiClientRepository apiClientRepository;
        private readonly ILogger<AuthenticationService> logger;
        private readonly JWTSettings _jwtSettings;
        private readonly ICacheProvider cacheProvider;

        public AuthenticationService(IApiClientRepository apiClientRepository, ILogger<AuthenticationService> logger, IOptions<JWTSettings> jwtSettings, ICacheProvider cacheProvider)
        {
            this.apiClientRepository = apiClientRepository;
            this.logger = logger;
            _jwtSettings = jwtSettings.Value;
            this.cacheProvider = cacheProvider;
        }

        public ApiClient AuthenticateClient(string hashToken)
        {
            var decodedValue = Convert.FromBase64String(hashToken);
            var values = Encoding.UTF8.GetString(decodedValue).Split("+");
            return AuthenticateClient(values[0], values[1]);
        }

        public ApiClient AuthenticateClient(string apiKey, string secret)
        {
            if (cacheProvider.TryRetrieve<ApiClient>($"api_{apiKey}", out ApiClient client)) {
                if (client.Secret != secret)
                {
                    logger.LogWarning("Authentication attempt with valid key " + apiKey + "but invalid secret " + secret);
                    return null;
                }
                return client;
            } else
            {
                var apiClient = apiClientRepository.GetApiClientByApiKey(apiKey);
                if (apiClient == null)
                {
                    logger.LogWarning("Authentication attempt with invalid API key " + apiKey);
                    return null;
                }

                if (apiClient.Secret != secret)
                {
                    logger.LogWarning("Authentication attempt with valid key " + apiKey + "but invalid secret " + secret);
                    return null;
                }
                cacheProvider.Store<ApiClient>($"api_{apiKey}", apiClient, DateTimeOffset.UtcNow.AddDays(10));
                return apiClient;
            }            
        }

        public async Task<ApiClient> AuthenticateClientAsync(string hashToken)
        {
            var decodedValue = Convert.FromBase64String(hashToken);
            var values = Encoding.UTF8.GetString(decodedValue).Split("+");
            return await AuthenticateClientAsync(values[0], values[1]);
        }
        public async Task<ApiClient> AuthenticateClientAsync(string apiKey, string secret)
        {
            var cachedClient = await cacheProvider.TryRetrieveAsync<ApiClient>($"api_{apiKey}");
            if (cachedClient != null)
            {
                if (cachedClient.Secret != secret)
                {
                    logger.LogWarning("Authentication attempt with valid key " + apiKey + "but invalid secret " + secret);
                    return null;
                }
                return cachedClient;
            }

            var apiClient = await apiClientRepository.GetApiClientByApiKeyAsync(apiKey);
            if (apiClient == null)
            {
                logger.LogWarning("Authentication attempt with invalid API key " + apiKey);
                return null;
            }

            if (apiClient.Secret != secret)
            {
                logger.LogWarning("Authentication attempt with valid key " + apiKey + "but invalid secret " + secret);
                return null;
            }

            await cacheProvider.StoreAsync<ApiClient>($"api_{apiKey}", apiClient, DateTimeOffset.UtcNow.AddDays(10));
            return apiClient;
        }

        public string GenerateJWToken(ApiClient apiClient)
        {
            var claims = new[]
            {
                new Claim("uid", apiClient.Id.ToString())
            };

            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        }
    }
}
