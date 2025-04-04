using System.Net;

namespace FileStore.Application.Common.Models.Responses
{
    public class AuthenticationResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string JwToken { get; set; }
    }
}
