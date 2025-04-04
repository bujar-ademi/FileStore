using System;
using System.Collections.Generic;
using System.Text;

namespace FileStore.Application.Common.Models.Responses
{
    public class FileResponse
    {
        public Guid Reference { get; set; }
        public string FileName { get; set; }
        public string FileUrl { get; set; }
    }
}
