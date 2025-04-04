using FileStore.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileStore.Domain.Entities
{
    public class FileType : EntityBase
    {
        public Guid ApiClientId { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public bool Allowed { get; set; }
    }
}
