using FileStore.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace FileStore.Application.Interfaces.Services
{
    public interface IAuthenticationService
    {
        ApiClient AuthenticateClient(string hashToken);
        ApiClient AuthenticateClient(string apiKey, string secret);
        Task<ApiClient> AuthenticateClientAsync(string hashToken);
        Task<ApiClient> AuthenticateClientAsync(string apiKey, string secret);
        string GenerateJWToken(ApiClient apiClient);
    }
}
