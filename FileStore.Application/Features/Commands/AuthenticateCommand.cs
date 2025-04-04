using FileStore.Application.Common.Models.Responses;
using FileStore.Application.Interfaces.Services;
using MediatR;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace FileStore.Application.Features.Commands
{
    public class AuthenticateCommand : IRequest<AuthenticationResponse>
    {
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; }
    }

    public class AuthenticateCommandHandler : IRequestHandler<AuthenticateCommand, AuthenticationResponse>
    {
        private readonly IAuthenticationService authenticationService;

        public AuthenticateCommandHandler(IAuthenticationService authenticationService)
        {
            this.authenticationService = authenticationService;
        }
        public async Task<AuthenticationResponse> Handle(AuthenticateCommand request, CancellationToken cancellationToken)
        {
            var apiClient = await authenticationService.AuthenticateClientAsync(request.ApiKey, request.ApiSecret);
            if (apiClient == null)
            {
                return new AuthenticationResponse
                {
                    StatusCode = HttpStatusCode.Unauthorized
                };
            }
            var jwToken = authenticationService.GenerateJWToken(apiClient);

            return new AuthenticationResponse
            {
                StatusCode = HttpStatusCode.OK,
                JwToken = jwToken
            };
        }
    }
}
