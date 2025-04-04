using FileStore.Application.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileStore.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base("Entity not found.") { }
        public NotFoundException(string message, IDictionary<string, object> args = null) : base(TemplateHelper.FillTemplate(message, args)) { }
    }
}
