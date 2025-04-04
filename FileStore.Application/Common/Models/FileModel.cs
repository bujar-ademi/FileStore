using FileStore.Application.Helpers;
using System;

namespace FileStore.Application.Common.Models
{
    public class FileModel
    {
        public Guid Reference { get; set; }
        public byte[] FileBytes { get; set; }
        public string ContentType { get; set; }
        public string FileName { get; set; }
        public DateTime DateCreated { get; set; }
        public string FileSize
        {
            get
            {
                if (FileBytes == null)
                {
                    return "";
                }
                return TemplateHelper.SizeSuffix(FileBytes.Length);
            }
        }
    }
}
