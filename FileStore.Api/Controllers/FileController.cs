using FileStore.Application.Features.Commands;
using FileStore.Application.Features.Query;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileStore.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class FileController : ApiController
    {

        //[Authorize]
        [HttpPost]
        [Route("")]
        public async Task<IActionResult> UploadFileAsync([FromForm] IFormFile file)
        {
            var result = await Mediator.Send(new UploadFileCommand { File = file }).ConfigureAwait(false);
            return Ok(result);
        }

        [HttpGet]
        [Route("{reference}")]
        public async Task<IActionResult> GetFileAsync(Guid reference)
        {
            var result = await Mediator.Send(new GetFileQuery { Reference = reference }).ConfigureAwait(false);

            return File(result.FileBytes, result.ContentType, result.FileName);
            //return new FileContentResult(result.FileBytes, result.ContentType);
        }

        [HttpDelete]
        [Route("{reference}")]
        public async Task<IActionResult> DeleteFileAsync(Guid reference)
        {
            var result = await Mediator.Send(new DeleteFileCommand { FileReference = reference }).ConfigureAwait(false);

            return Ok(result);
        }

        [HttpGet]
        public IActionResult GetTestFile()
        {
            return Ok();
        }
    }
}
