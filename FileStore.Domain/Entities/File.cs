using FileStore.Domain.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileStore.Domain.Entities
{
    public class File : EntityBase
    {
        public Guid Reference { get; set; }
        public Guid FileTypeId { get; set; }
        public Guid ApiClientId { get; set; }
        public string FileName { get; set; }
        public string FullPath { get; set; }
        public long Size { get; set; }
        public DateTime DateCreated { get; set; }
        public bool MarkedForDeletion { get; set; }
    }
}
