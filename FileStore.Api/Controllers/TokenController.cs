using FileStore.Application.Features.Commands;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FileStore.Api.Controllers
{
    [Produces("application/json")]
    [Route("[controller]")]
    [ApiController]
    public class TokenController : ApiController
    {
        [Route("authenticate")]
        [HttpPost]
        [ProducesResponseType(200)]
        [ProducesResponseType(401)]
        public async Task<IActionResult> Authenticate([FromBody] AuthenticateCommand request)
        {
            var response = await Mediator.Send(request).ConfigureAwait(false);
            if (response.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return Unauthorized();
            }
            return Ok(response.JwToken);
        }
    }
}
